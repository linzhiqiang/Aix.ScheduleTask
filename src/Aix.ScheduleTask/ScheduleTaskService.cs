using Aix.ScheduleTask.Executors;
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
        private readonly ScheduleTaskExecutor _scheduleTaskExecutor;
        private readonly ExpireLogExecutor _expireLogExecutor;
        private readonly ErrorTaskExecutor _errorTaskExecutor;

        private ConcurrentDictionary<string, CrontabSchedule> CrontabScheduleCache = new ConcurrentDictionary<string, CrontabSchedule>();
        //private volatile bool _isStart = false;
        private RepeatChecker _repeatStartChecker = new RepeatChecker();
        private RepeatChecker _repeatStopChecker = new RepeatChecker();
        // private CancellationToken StopingToken;

        private int PreReadSecond = 10; //提前读取多长数据
        public event Func<ScheduleTaskContext, Task> OnHandleMessage
        {
            add { _scheduleTaskExecutor.OnHandleMessage += value; }
            remove { _scheduleTaskExecutor.OnHandleMessage -= value; }
        }

        readonly string ScheduleTaskLock = "AixScheduleTaskLock";

        public ScheduleTaskService(ILogger<ScheduleTaskService> logger,
            AixScheduleTaskOptions options,
            IScheduleTaskLifetime scheduleTaskLifetime,
            IAixScheduleTaskRepository aixScheduleTaskRepository,
            IAixDistributionLockRepository aixDistributionLockRepository,
            IScheduleTaskDistributedLock scheduleTaskDistributedLock,
            IAixScheduleTaskLogRepository aixScheduleTaskLogRepository,
            MyMultithreadTaskExecutor taskExecutor,
            ScheduleTaskExecutor scheduleTaskExecutor,
            ExpireLogExecutor expireLogExecutor,
            ErrorTaskExecutor errorTaskExecutor
            )
        {
            _logger = logger;
            _options = options;
            _scheduleTaskLifetime = scheduleTaskLifetime;
            PreReadSecond = _options.PreReadSecond;
            _aixScheduleTaskRepository = aixScheduleTaskRepository;
            _aixDistributionLockRepository = aixDistributionLockRepository;
            _scheduleTaskDistributedLock = scheduleTaskDistributedLock;
            _aixScheduleTaskLogRepository = aixScheduleTaskLogRepository;
            _taskExecutor = taskExecutor;
            _scheduleTaskExecutor = scheduleTaskExecutor;
            _expireLogExecutor = expireLogExecutor;
            _errorTaskExecutor = errorTaskExecutor;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <returns></returns>
        public Task Start()
        {
            if (!_repeatStartChecker.Check()) return Task.CompletedTask;
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Init();
                    await _expireLogExecutor.Start();
                    await _errorTaskExecutor.Start();
                    await StartProcessTask();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Aix.ScheduleTask定时任务启动出错");
                }

            }, TaskCreationOptions.LongRunning);
            _scheduleTaskLifetime.NotifyStarted();
            return Task.CompletedTask;
        }

        private async Task StartProcessTask()
        {
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

                var Schedule = CrontabHelper.ParseCron(_logger,task);
                if (task.LastExecuteTime == 0) task.LastExecuteTime = now; //任务第一次执行时，从当前时间开始
                var nextExecuteTimeSpan = CrontabHelper.GetNextDueTime(Schedule, DateTimeUtils.TimeStampToDateTime(task.LastExecuteTime), DateTimeUtils.TimeStampToDateTime(now));

                if (nextExecuteTimeSpan <= TimeSpan.Zero) //时间到了，开始执行任务
                {
                    if (nextExecuteTimeSpan >= TimeSpan.FromSeconds(0 - PreReadSecond))//排除过期太久的，（服务停了好久，再启动这些就不执行了）
                    {
                        await _scheduleTaskExecutor.Trigger(task, TimeSpan.Zero); //
                    }

                    now = DateTimeUtils.GetTimeStamp();
                    task.LastExecuteTime = now;
                    //计算下一次执行时间
                    nextExecuteTimeSpan = CrontabHelper.GetNextDueTime(Schedule, DateTimeUtils.TimeStampToDateTime(task.LastExecuteTime), DateTimeUtils.TimeStampToDateTime(now));
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

        /// <summary>
        ///  保存执行结果
        /// </summary>
        /// <param name="resultDTO"></param>
        /// <returns></returns>
        public Task SaveExecuteResult(ExecuteResultDTO resultDTO)
        {
            _taskExecutor.Execute(async (state) =>
            {
                AixScheduleTaskLog log = new AixScheduleTaskLog
                {
                    Id = resultDTO.LogId,
                    ResultCode = resultDTO.Code == 0 ? (int)OPStatus.Success : (int)OPStatus.Fail, //状态 0=初始化 /*1=执行中*/ 2=执行成功 9=执行失败  
                    ResultMessage = StringUtils.SubString($"{resultDTO.Code},{resultDTO.Message}", _options.LogResultMessageMaxLength > 0 ? _options.LogResultMessageMaxLength : 500),
                    ResultTime = DateTime.Now,
                    ModifyTime = DateTime.Now
                };
                await _aixScheduleTaskLogRepository.UpdateAsync(log);
            }, null);

            return Task.CompletedTask;

        }

        /// <summary>
        /// 释放资源
        /// </summary>
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
            }
        }

        #endregion
    }
}
