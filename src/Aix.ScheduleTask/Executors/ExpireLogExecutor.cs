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
    /// 清除过期日志
    /// </summary>
    public class ExpireLogExecutor
    {
        ILogger<ExpireLogExecutor> _logger;
        private readonly IAixScheduleTaskLogRepository _aixScheduleTaskLogRepository;
        private readonly AixScheduleTaskOptions _options;
        private readonly IScheduleTaskLifetime _scheduleTaskLifetime;
        public ExpireLogExecutor(ILogger<ExpireLogExecutor> logger,
            IAixScheduleTaskLogRepository aixScheduleTaskLogRepository,
            AixScheduleTaskOptions aixScheduleTaskOptions,
            IScheduleTaskLifetime scheduleTaskLifetime
            )
        {
            _logger = logger;
            _aixScheduleTaskLogRepository = aixScheduleTaskLogRepository;
            _options = aixScheduleTaskOptions;
            _scheduleTaskLifetime = scheduleTaskLifetime;
        }

        /// <summary>
        /// 清除过期日志
        /// </summary>
        /// <returns></returns>
        public Task Start()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                var logExpireHour = _options.LogExpireHour > 0 ? _options.LogExpireHour : 168;
                while (!_scheduleTaskLifetime.ScheduleTaskStopping.IsCancellationRequested)
                {
                    _scheduleTaskLifetime.ScheduleTaskStopping.ThrowIfCancellationRequested();
                    try
                    {
                        var expiration = DateTime.Now.AddHours(0 - logExpireHour);
                        await _aixScheduleTaskLogRepository.Delete(expiration);
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError(ex, "Aix.ScheduleTask任务取消");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "清除过期日志异常");
                    }
                    await TaskEx.DelayNoException(TimeSpan.FromHours(2), _scheduleTaskLifetime.ScheduleTaskStopping);
                }
            });

            return Task.CompletedTask;

        }
    }
}
