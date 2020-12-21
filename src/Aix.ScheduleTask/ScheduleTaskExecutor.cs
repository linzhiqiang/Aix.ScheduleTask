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

    比如每天9点半执行，如果9点半这个时刻你停了一会，9点半后重新启动， 这时会重新执行一次的。
     */

    public interface IScheduleTaskExecutor : IDisposable
    {
        Task Start();

        event Func<ScheduleTaskContext, Task> OnHandleMessage;
    }

    /// <summary>
    /// 定时任务执行器
    /// </summary>
    public class ScheduleTaskExecutor : IScheduleTaskExecutor
    {
        ILogger<ScheduleTaskExecutor> _logger;
        private readonly IAixScheduleTaskRepository _aixScheduleTaskRepository;
        private readonly IAixDistributionLockRepository _aixDistributionLockRepository;
        private readonly IScheduleTaskDistributedLock _scheduleTaskDistributedLock;
        private readonly AixScheduleTaskOptions _options;

        private ConcurrentDictionary<string, CrontabSchedule> CrontabScheduleCache = new ConcurrentDictionary<string, CrontabSchedule>();
        //private volatile bool _isStart = false;
        private RepeatChecker _repeatStartChecker = new RepeatChecker();
        private RepeatChecker _repeatStopChecker = new RepeatChecker();
        private readonly CancellationTokenSource _startedSource = new CancellationTokenSource();
        private CancellationToken StartedToken => _startedSource.Token;

        private int CrontabIntervalSecond = 30; //没有数据时等待时间
        public event Func<ScheduleTaskContext, Task> OnHandleMessage;

        readonly string ScheduleTaskLock = "ScheduleTaskLock";

        public ScheduleTaskExecutor(ILogger<ScheduleTaskExecutor> logger,
            AixScheduleTaskOptions options,
            IAixScheduleTaskRepository aixScheduleTaskRepository,
            IAixDistributionLockRepository aixDistributionLockRepository,
            IScheduleTaskDistributedLock scheduleTaskDistributedLock
            )
        {
            _logger = logger;
            _options = options;
            CrontabIntervalSecond = _options.CrontabIntervalSecond;
            _aixScheduleTaskRepository = aixScheduleTaskRepository;
            _aixDistributionLockRepository = aixDistributionLockRepository;
            _scheduleTaskDistributedLock = scheduleTaskDistributedLock;
        }

        public Task Start()
        {
            if (!_repeatStartChecker.Check()) return Task.CompletedTask;
            _logger.LogInformation("开始执行定时任务......");
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await InnerStart();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "定时任务启动出错");
                }

            }, TaskCreationOptions.LongRunning);

            return Task.CompletedTask;
        }

        private async Task InnerStart()
        {
            await Init();
            while (!StartedToken.IsCancellationRequested)
            {
                try
                {
                    await DistributionLockWrap();
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower().IndexOf("timeout") < 0)
                    {
                        _logger.LogError(ex, "定时任务执行出错");
                        await Task.Delay(TimeSpan.FromSeconds(5), StartedToken);
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
               }, () =>
               {
                   return Task.CompletedTask;
               });

            var depay = nextExecuteDelays.Any() ? nextExecuteDelays.Min() : TimeSpan.FromSeconds(CrontabIntervalSecond);
            if (depay > TimeSpan.FromSeconds(CrontabIntervalSecond)) depay = TimeSpan.FromSeconds(CrontabIntervalSecond);
            await Task.Delay(depay, StartedToken);
        }

        private async Task<List<TimeSpan>> Execute()
        {
            List<TimeSpan> nextExecuteDelays = new List<TimeSpan>(); //记录每个任务的下次执行时间，取最小的等待

            var now = DateTimeUtils.GetTimeStamp();
            var taskList = await _aixScheduleTaskRepository.QueryAllEnabled(CrontabIntervalSecond * 1000 + now);
            //处理
            foreach (var task in taskList)
            {
                if (StartedToken.IsCancellationRequested) break;
                try
                {
                    var Schedule = ParseCron(task.Cron);
                    if (task.LastExecuteTime == 0) task.LastExecuteTime = now;
                    var nextExecuteTimeSpan = GetNextDueTime(Schedule, TimeStampToDateTime(task.LastExecuteTime), TimeStampToDateTime(now));
                    if (nextExecuteTimeSpan.TotalMilliseconds <= 0) //时间到了，开始执行任务
                    {
                        await HandleMessage(task); //建议插入任务队列

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
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"定时任务解析出错 {task.Id},{task.TaskName},{task.Cron}");
                }
            }

            return nextExecuteDelays;
        }

        private Task HandleMessage(AixScheduleTaskInfo taskInfo)
        {
            if (OnHandleMessage == null) return Task.CompletedTask;
            //把线程队列引用过来，根据id进入不同的线程队列，保证串行执行

            // _logger.LogDebug($"执行定时任务:{taskInfo.Id},{taskInfo.TaskName},{taskInfo.ExecutorParam}");
            Task.Run(async () =>
            {
                //_aixScheduleTaskRepository.OpenNewContext();
                try
                {
                    await OnHandleMessage(new ScheduleTaskContext { Id = taskInfo.Id, ExecutorParam = taskInfo.ExecutorParam });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"定时任务执行出错 {taskInfo.Id},{taskInfo.TaskName},{taskInfo.ExecutorParam}");
                }

            });

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (!_repeatStopChecker.Check()) return;
            _logger.LogInformation("结束执行定时任务......");
            NotifyStopped();
        }

        #region private 
        private void NotifyStopped()
        {
            try
            {
                // Noop if this is already cancelled
                if (_startedSource.IsCancellationRequested)
                {
                    return;
                }

                // Run the cancellation token callbacks
                _startedSource.Cancel(throwOnFirstException: false);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred stopping the scheduletask", ex);
            }
        }

        private CrontabSchedule ParseCron(string cron)
        {
            CrontabSchedule result;
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
            return result;
        }

        private static TimeSpan GetNextDueTime(CrontabSchedule Schedule, DateTime LastDueTime, DateTime now)
        {
            var nextOccurrence = Schedule.GetNextOccurrence(LastDueTime);
            TimeSpan dueTime = nextOccurrence - now;// DateTime.Now;

            if (dueTime.TotalMilliseconds <= 0)
            {
                dueTime = TimeSpan.Zero;
            }

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
                _logger.LogError(ex, "定时任务初始化出错");
                throw;
            }
        }

        #endregion
    }
}
