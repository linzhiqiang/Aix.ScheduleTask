using Aix.ScheduleTask.Foundation;
using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Repository;
using Aix.ScheduleTask.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Executors
{
    /// <summary>
    /// 触发任务 进入任务队列等待调度
    /// </summary>
   public class ScheduleTaskExecutor
    {
        ILogger<ScheduleTaskExecutor> _logger;
        private readonly IAixScheduleTaskLogRepository _aixScheduleTaskLogRepository;
        private readonly AixScheduleTaskOptions _options;
        private readonly MyMultithreadTaskExecutor _taskExecutor;

        public event Func<ScheduleTaskContext, Task> OnHandleMessage;

        public ScheduleTaskExecutor(ILogger<ScheduleTaskExecutor> logger,
            MyMultithreadTaskExecutor taskExecutor,
            IAixScheduleTaskLogRepository aixScheduleTaskLogRepository,
            AixScheduleTaskOptions aixScheduleTaskOptions
            )
        {
            _logger = logger;
            _taskExecutor = taskExecutor;
            _aixScheduleTaskLogRepository = aixScheduleTaskLogRepository;
            _options = aixScheduleTaskOptions;

        }
        public Task Trigger(AixScheduleTaskInfo taskInfo, TimeSpan delay)
        {
            if (OnHandleMessage == null) return Task.CompletedTask;
            //把线程队列引用过来，根据id进入不同的线程队列，保证串行执行

            // _logger.LogDebug($"执行定时任务:{taskInfo.Id},{taskInfo.TaskName},{taskInfo.ExecutorParam}");

            _taskExecutor.GetSingleThreadTaskExecutor(taskInfo.Id).Schedule(async (state) =>
            {
                var innerTaskInfo = (AixScheduleTaskInfo)state;
                int logId = 0;
                try
                {
                    logId = await SaveLog(innerTaskInfo);
                    await OnHandleMessage(new ScheduleTaskContext { LogId = logId, TaskId = innerTaskInfo.Id, TaskGroup = innerTaskInfo.TaskGroup, TaskContent = innerTaskInfo.TaskContent });
                    await UpdateTriggerCode(logId, OPStatus.Success, "success");
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex, "Aix.ScheduleTask任务取消");
                    await UpdateTriggerCode(logId, OPStatus.Fail, ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Aix.ScheduleTask定时任务执行出错 {innerTaskInfo.Id},{innerTaskInfo.TaskName},{innerTaskInfo.TaskContent}");
                    await UpdateTriggerCode(logId, OPStatus.Fail, ex.Message);
                }
            }, taskInfo, delay);

            return Task.CompletedTask;
        }

        private async Task<int> SaveLog(AixScheduleTaskInfo taskInfo)
        {
            AixScheduleTaskLog log = new AixScheduleTaskLog
            {
                ScheduleTaskId = taskInfo.Id,
                RetryCount = taskInfo.MaxRetryCount,
                TriggerCode = (int)OPStatus.Init,
                TriggerMessage = "",
                TriggerTime = DateTime.Now,
                ResultCode = (int)OPStatus.Init,
                ResultMessage = "",// StringUtils.SubString(resultDTO.Message, 500),
                AlarmStatus = (sbyte)AlarmStatus.Init,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };
            var newLogId = await _aixScheduleTaskLogRepository.InsertAsync(log);
            return (int)newLogId;
        }

        private Task UpdateTriggerCode(int logId, OPStatus triggerCode, string triggerMessage)
        {
            _taskExecutor.Execute(async (state) =>
            {
                AixScheduleTaskLog log = new AixScheduleTaskLog
                {
                    Id = logId,
                    TriggerCode = (int)triggerCode,
                    TriggerMessage = StringUtils.SubString(triggerMessage, _options.LogResultMessageMaxLength > 0 ? _options.LogResultMessageMaxLength : 500),
                    ModifyTime = DateTime.Now
                };
                await _aixScheduleTaskLogRepository.UpdateAsync(log);
            }, null);

            return Task.CompletedTask;
        }
    }
}
