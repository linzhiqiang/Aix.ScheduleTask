using Aix.ScheduleTask.Foundation;
using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Repository;
using Aix.ScheduleTask.Utils;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aix.ScheduleTask
{
    /*
     * * * * * * 
     sec (0 - 59)
    min (0 - 59)
    hour (0 - 23)
    day of month (1 - 31)
    month (1 - 12)
    day of week (0 - 6) (Sunday=0)

    比如每天9点半执行，如果9点半这个时刻你停了一会，9点半后重新启动， 在{PreReadSecond}秒内继续执行，否则不执行(可能停机太久了，没必要执行了)
     */


    /// <summary>
    /// 定时任务执行器  不建议间隔10秒以下的的定时任务
    /// </summary>
    public class ScheduleTaskService : IScheduleTaskService
    {
        ILogger<ScheduleTaskService> _logger;
        private readonly IAixScheduleTaskRepository _aixScheduleTaskRepository;
        private readonly IAixDistributionLockRepository _aixDistributionLockRepository;
        private readonly IScheduleTaskDistributedLock _scheduleTaskDistributedLock;
        private readonly IAixScheduleTaskLogRepository _aixScheduleTaskLogRepository;
        private readonly AixScheduleTaskOptions _options;
        private readonly IScheduleTaskLifetime _scheduleTaskLifetime;
        private readonly MyMultithreadTaskExecutor _taskExecutor;

        private ConcurrentDictionary<string, CrontabSchedule> CrontabScheduleCache = new ConcurrentDictionary<string, CrontabSchedule>();
        //private volatile bool _isStart = false;
        private RepeatChecker _repeatStartChecker = new RepeatChecker();
        private RepeatChecker _repeatStopChecker = new RepeatChecker();
        // private CancellationToken StopingToken;

        private int PreReadSecond = 10; //提前读取多长数据
        public event Func<ScheduleTaskContext, Task> OnHandleMessage;

        readonly string ScheduleTaskLock = "AixScheduleTaskLock";

        public ScheduleTaskService(ILogger<ScheduleTaskService> logger,
            AixScheduleTaskOptions options,
            IScheduleTaskLifetime scheduleTaskLifetime,
            IAixScheduleTaskRepository aixScheduleTaskRepository,
            IAixDistributionLockRepository aixDistributionLockRepository,
            IScheduleTaskDistributedLock scheduleTaskDistributedLock,
            IAixScheduleTaskLogRepository aixScheduleTaskLogRepository,
            MyMultithreadTaskExecutor taskExecutor
            )
        {
            _logger = logger;
            _options = options;
            _scheduleTaskLifetime = scheduleTaskLifetime;
            // StopingToken = _scheduleTaskLifetime.ScheduleTaskStopping;
            PreReadSecond = _options.PreReadSecond;
            _aixScheduleTaskRepository = aixScheduleTaskRepository;
            _aixDistributionLockRepository = aixDistributionLockRepository;
            _scheduleTaskDistributedLock = scheduleTaskDistributedLock;
            _aixScheduleTaskLogRepository = aixScheduleTaskLogRepository;
            _taskExecutor = taskExecutor;
        }

        public Task Start()
        {
            if (!_repeatStartChecker.Check()) return Task.CompletedTask;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await InnerStart();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Aix.ScheduleTask定时任务启动出错");
                }

            }, TaskCreationOptions.LongRunning);
            _scheduleTaskLifetime.NotifyStarted();
            return Task.CompletedTask;
        }

        private async Task InnerStart()
        {
            await Init();
            await ScheduleDeleteExpireLog();
            while (!_scheduleTaskLifetime.ScheduleTaskStopping.IsCancellationRequested)
            {
                try
                {
                    await DistributionLockWrap();
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex, "Aix.ScheduleTask定时任务取消");
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().IndexOf("timeout") < 0)
                    {
                        _logger.LogError(ex, "Aix.ScheduleTask定时任务执行出错");
                        await TaskEx.DelayNoException(TimeSpan.FromSeconds(5), _scheduleTaskLifetime.ScheduleTaskStopping);
                    }
                }

            }
        }

        /// <summary>
        /// 根据是否集群环境是否增加分布式锁
        /// </summary>
        /// <returns></returns>
        private async Task DistributionLockWrap()
        {
            List<TimeSpan> nextExecuteDelays = null;
            await _scheduleTaskDistributedLock.Lock(ScheduleTaskLock, TimeSpan.FromSeconds(300), async () =>
               {
                   nextExecuteDelays = await Execute();
               }, async () =>
               {
                   //出现并发 休息一会，说明其他服务器已经在执行了
                   await TaskEx.DelayNoException(TimeSpan.FromSeconds(PreReadSecond), _scheduleTaskLifetime.ScheduleTaskStopping);
               });

            var depay = nextExecuteDelays.Any() ? nextExecuteDelays.Min() : TimeSpan.FromSeconds(PreReadSecond);
            if (depay > TimeSpan.FromSeconds(PreReadSecond)) depay = TimeSpan.FromSeconds(PreReadSecond);
            await TaskEx.DelayNoException(depay, _scheduleTaskLifetime.ScheduleTaskStopping);
        }

        private async Task<List<TimeSpan>> Execute()
        {
            List<TimeSpan> nextExecuteDelays = new List<TimeSpan>(); //记录每个任务的下次执行时间，取最小的等待

            if (_taskExecutor.GetTaskCount() > _options.Backlog)
            {
                _logger.LogWarning($"Aix.ScheduleTask定时任务积压太多超过{_options.Backlog}条定时任务待处理");
                await Task.Delay(TimeSpan.FromSeconds(5));
                return nextExecuteDelays;
            }

            var now = DateTimeUtils.GetTimeStamp();
            var taskList = await _aixScheduleTaskRepository.QueryAllEnabled(PreReadSecond * 1000 + now); //拉取即将到期的任务 提前{PreReadSecond}秒
            //逐个处理
            foreach (var task in taskList)
            {
                _scheduleTaskLifetime.ScheduleTaskStopping.ThrowIfCancellationRequested();

                var Schedule = ParseCron(task);
                if (task.LastExecuteTime == 0) task.LastExecuteTime = now; //任务第一次执行时，从当前时间开始
                var nextExecuteTimeSpan = GetNextDueTime(Schedule, TimeStampToDateTime(task.LastExecuteTime), TimeStampToDateTime(now));

                if (nextExecuteTimeSpan <= TimeSpan.Zero) //时间到了，开始执行任务
                {
                    if (nextExecuteTimeSpan >= TimeSpan.FromSeconds(0 - PreReadSecond))//排除过期太久的，（服务停了好久，再启动这些就不执行了）
                    {
                        await HandleMessage(task); //建议插入任务队列
                    }

                    now = DateTimeUtils.GetTimeStamp();
                    task.LastExecuteTime = now;
                    //计算下一次执行时间
                    nextExecuteTimeSpan = GetNextDueTime(Schedule, TimeStampToDateTime(task.LastExecuteTime), TimeStampToDateTime(now));
                    task.NextExecuteTime = now + (long)nextExecuteTimeSpan.TotalMilliseconds;
                    task.ModifyTime = DateTime.Now;
                    await _aixScheduleTaskRepository.UpdateAsync(task);
                }

                if (task.NextExecuteTime == 0)  //只有第一次且未执行时更新下即可
                {
                    task.NextExecuteTime = now + (long)nextExecuteTimeSpan.TotalMilliseconds;
                    task.ModifyTime = DateTime.Now;
                    await _aixScheduleTaskRepository.UpdateAsync(task);
                }

                nextExecuteDelays.Add(nextExecuteTimeSpan);

            }

            return nextExecuteDelays;
        }

        private Task HandleMessage(AixScheduleTaskInfo taskInfo)
        {
            if (OnHandleMessage == null) return Task.CompletedTask;
            //把线程队列引用过来，根据id进入不同的线程队列，保证串行执行

            // _logger.LogDebug($"执行定时任务:{taskInfo.Id},{taskInfo.TaskName},{taskInfo.ExecutorParam}");

            _taskExecutor.GetSingleThreadTaskExecutor(taskInfo.Id).Execute(async (state) =>
            {
                var innerTaskInfo = (AixScheduleTaskInfo)state;
                try
                {
                    var logId = await SaveLog(innerTaskInfo);
                    await OnHandleMessage(new ScheduleTaskContext { Id = logId, TaskGroup = innerTaskInfo.TaskGroup, TaskContent = innerTaskInfo.TaskContent });
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex, "Aix.ScheduleTask任务取消");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Aix.ScheduleTask定时任务执行出错 {innerTaskInfo.Id},{innerTaskInfo.TaskName},{innerTaskInfo.TaskContent}");
                }
            }, taskInfo);

            return Task.CompletedTask;
        }

        private async Task<int> SaveLog(AixScheduleTaskInfo taskInfo)
        {
            AixScheduleTaskLog log = new AixScheduleTaskLog
            {
                ScheduleTaskId = taskInfo.Id,
                Status = 1,
                ResultCode = 0,
                ResultMessage = "",// StringUtils.SubString(resultDTO.Message, 500),
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };
            var newLogId = await _aixScheduleTaskLogRepository.InsertAsync(log);
            return (int)newLogId;
        }

        public Task SaveExecuteResult(ExecuteResultDTO resultDTO)
        {
            _taskExecutor.Execute(async (state) =>
            {
                AixScheduleTaskLog log = new AixScheduleTaskLog
                {
                    Id = resultDTO.Id,
                    Status = resultDTO.Code == 0 ? 2 : 9, //状态 0=初始化 1=执行中 2=执行成功 9=执行失败  
                    ResultCode = resultDTO.Code,
                    ResultMessage = StringUtils.SubString(resultDTO.Message, _options.LogResultMessageMaxLength > 0 ? _options.LogResultMessageMaxLength : 500),
                    ModifyTime = DateTime.Now
                };
                await _aixScheduleTaskLogRepository.UpdateAsync(log);
            }, null);

            return Task.CompletedTask;

        }

        public void Dispose()
        {
            if (!_repeatStopChecker.Check()) return;

            //开始stop
            _scheduleTaskLifetime.Stop();

            //开始stop 具体需要stop的

            //stop结束通知
            _scheduleTaskLifetime.NotifyStopped();
        }

        #region private 

        private CrontabSchedule ParseCron(AixScheduleTaskInfo task)
        {
            string cron = task.Cron;
            CrontabSchedule result = null;
            try
            {
                if (string.IsNullOrEmpty(cron)) throw new Exception($"Aix.ScheduleTask任务{task.Id},{task.TaskName}表达式配置为空");
                if (CrontabScheduleCache.TryGetValue(cron, out result))
                {
                    return result;
                }
                var options = new CrontabSchedule.ParseOptions
                {
                    IncludingSeconds = cron.Split(' ').Length > 5,
                };
                result = CrontabSchedule.Parse(cron, options);
                CrontabScheduleCache.TryAdd(cron, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Aix.ScheduleTask定时任务解析出错 {task.Id},{task.TaskName},{task.Cron}");
            }
            return result;
        }

        private DateTime GetNexeTime(CrontabSchedule Schedule, DateTime LastDueTime)
        {
            return Schedule.GetNextOccurrence(LastDueTime);
        }

        private TimeSpan GetNextDueTime(CrontabSchedule Schedule, DateTime LastDueTime, DateTime now)
        {
            var nextOccurrence = GetNexeTime(Schedule, LastDueTime);
            TimeSpan dueTime = nextOccurrence - now;// DateTime.Now;

            //if (dueTime.TotalMilliseconds <= 0)
            //{
            //    dueTime = TimeSpan.Zero;
            //}

            return dueTime;
        }



        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private static DateTime TimeStampToDateTime(long timestamp)
        {
            return DateTimeUtils.TimeStampToDateTime(timestamp);
        }

        private async Task Init()
        {
            try
            {
                var model = await _aixDistributionLockRepository.Get(ScheduleTaskLock);
                if (model == null)
                {
                    await _aixDistributionLockRepository.InsertAsync(new AixDistributionLock { LockName = ScheduleTaskLock });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aix.ScheduleTask定时任务初始化出错");
                throw;


            }
        }

        /// <summary>
        /// 清除过期日志
        /// </summary>
        /// <returns></returns>
        private  Task ScheduleDeleteExpireLog()
        {
            Task.Run(async() =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                var logExpireHour = _options.LogExpireHour > 0 ? _options.LogExpireHour : 168 ;
                while (true)
                {
                    _scheduleTaskLifetime.ScheduleTaskStopping.ThrowIfCancellationRequested();
                    try
                    {
                        var expiration = DateTime.Now.AddHours(0 - logExpireHour);
                        await _aixScheduleTaskLogRepository.Delete(expiration);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "清除过期日志异常");
                    }
                    await TaskEx.DelayNoException(TimeSpan.FromHours(1), _scheduleTaskLifetime.ScheduleTaskStopping);
                }
            });

            return Task.CompletedTask;
            
        }

        private void WarnIfDataTooLarge(int count)
        {
            if (count >= 2000)
            {
                _logger.LogWarning("*******************************************");
                _logger.LogWarning("定时任务数量太大，需要优化为分页轮询");
                _logger.LogWarning("*******************************************");
            }
        }

        #endregion
    }
}
