using Aix.ScheduleTask.Foundation;
using Aix.ScheduleTask.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Executors
{
    /// <summary>
    /// 失败的任务 进行重试处理
    /// </summary>
   public class ErrorTaskExecutor
    {
        ILogger<ErrorTaskExecutor> _logger;
        private readonly IAixScheduleTaskRepository _aixScheduleTaskRepository;
        private readonly IAixScheduleTaskLogRepository _aixScheduleTaskLogRepository;
        private readonly AixScheduleTaskOptions _options;
        private readonly IScheduleTaskLifetime _scheduleTaskLifetime;
        private readonly ScheduleTaskExecutor _scheduleTaskExecutor;
        public ErrorTaskExecutor(ILogger<ErrorTaskExecutor> logger,
            IAixScheduleTaskRepository aixScheduleTaskRepository,
            IAixScheduleTaskLogRepository aixScheduleTaskLogRepository,
            AixScheduleTaskOptions aixScheduleTaskOptions,
            IScheduleTaskLifetime scheduleTaskLifetime,
            ScheduleTaskExecutor scheduleTaskExecutor
            )
        {
            _logger = logger;
            _aixScheduleTaskRepository = aixScheduleTaskRepository;
            _aixScheduleTaskLogRepository = aixScheduleTaskLogRepository;
            _options = aixScheduleTaskOptions;
            _scheduleTaskLifetime = scheduleTaskLifetime;
            _scheduleTaskExecutor = scheduleTaskExecutor;
        }


        /// <summary>
        /// 定时处理 执行错误的任务，进行重试
        /// </summary>
        /// <returns></returns>
        public Task Start()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                while (!_scheduleTaskLifetime.ScheduleTaskStopping.IsCancellationRequested)
                {
                    _scheduleTaskLifetime.ScheduleTaskStopping.ThrowIfCancellationRequested();
                    try
                    {
                        await ScheduleProcessErrorTask();
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError(ex, "Aix.ScheduleTask任务取消");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理失败任务异常");
                    }
                    await TaskEx.DelayNoException(TimeSpan.FromSeconds(10), _scheduleTaskLifetime.ScheduleTaskStopping);
                }
            });

            return Task.CompletedTask;
        }

        private async Task ScheduleProcessErrorTask()
        {
            var logIds = await _aixScheduleTaskLogRepository.QueryFailJobLogIds(1000);

            foreach (var logId in logIds)
            {
                int lockRet = await _aixScheduleTaskLogRepository.UpdateAlarmStatus(logId, 0, -1);
                if (lockRet < 1) continue;//cas锁

                var logInfo = await _aixScheduleTaskLogRepository.GetById(logId);
                var taskInfo = await _aixScheduleTaskRepository.GetById(logInfo.ScheduleTaskId);

                if (logInfo.RetryCount > 0)
                {
                    //10秒后执行
                    taskInfo.MaxRetryCount = logInfo.RetryCount - 1;

                    var diff = logInfo.CreateTime.AddSeconds(_options.RetryIntervalMillisecond) - DateTime.Now;
                    await _scheduleTaskExecutor.Trigger( taskInfo, diff); //建议插入任务队列
                }

                await _aixScheduleTaskLogRepository.UpdateAlarmStatus(logId, -1, 1);
            }

        }
    }
}
