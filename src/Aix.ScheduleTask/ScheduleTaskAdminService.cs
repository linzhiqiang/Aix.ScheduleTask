using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Repository;
using Aix.ScheduleTask.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask
{
    /// <summary>
    /// 任务管理接口
    /// </summary>
    public interface IScheduleTaskAdminService
    {
        /// <summary>
        /// 保存任务 有id是修改，否则新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveTask(ScheduleTaskDTO model);

        Task DeleteTask(int taskId);

        Task EnableTask(int taskId);

        Task DisableTask(int taskId);
    }
    public class ScheduleTaskAdminService : IScheduleTaskAdminService
    {
        ILogger<ScheduleTaskAdminService> _logger;
        private readonly IAixScheduleTaskRepository _aixScheduleTaskRepository;
        public ScheduleTaskAdminService(ILogger<ScheduleTaskAdminService> logger,
            IAixScheduleTaskRepository aixScheduleTaskRepository
            )
        {
            _logger = logger;
            _aixScheduleTaskRepository = aixScheduleTaskRepository;
        }
        public async Task<int> SaveTask(ScheduleTaskDTO req)
        {
            AssertUtils.IsNotEmpty(req.TaskName, $"{nameof(AixScheduleTaskInfo.TaskName)}为空");
            AssertUtils.IsNotEmpty(req.Cron, $"{nameof(AixScheduleTaskInfo.Cron)}为空");
            AssertUtils.IsNotEmpty(req.TaskContent, $"{nameof(AixScheduleTaskInfo.TaskContent)}为空");
            AssertUtils.IsTrue(req.MaxRetryCount >= 0, $"{nameof(AixScheduleTaskInfo.MaxRetryCount)} 为空");

            var model = await _aixScheduleTaskRepository.GetById(req.TaskId);
            if (model == null)
            {
                model = new AixScheduleTaskInfo
                {
                    TaskGroup = req.TaskGroup ?? "",
                    TaskStatus = 1,
                    TaskName = req.TaskName,
                    TaskDesc = req.TaskDesc ?? "",
                    Cron = req.Cron,
                    TaskContent = req.TaskContent ?? "",
                    LastExecuteTime = 0,
                    NextExecuteTime = 0,
                    MaxRetryCount = req.MaxRetryCount,
                    CreatorId = req.UserId ?? "",
                    CreateTime = DateTime.Now,
                    ModifierId = req.UserId ?? "",
                    ModifyTime = DateTime.Now
                };

                var newId = await _aixScheduleTaskRepository.InsertAsync(model);
                model.Id = (int)newId;
            }

            else
            {
                model = new AixScheduleTaskInfo
                {
                    Id = req.TaskId,
                    TaskGroup = req.TaskGroup ?? "",
                    //TaskStatus = 1,
                    TaskName = req.TaskName,
                    TaskDesc = req.TaskDesc ?? "",
                    Cron = req.Cron,
                    TaskContent = req.TaskContent ?? "",
                    // LastExecuteTime = 0,
                    // NextExecuteTime = 0,
                    MaxRetryCount = req.MaxRetryCount,
                    // CreatorId = req.UserId ?? "",
                    // CreateTime = DateTime.Now,
                    ModifierId = req.UserId ?? "",
                    ModifyTime = DateTime.Now
                };
                await _aixScheduleTaskRepository.UpdateAsync(model);
            }
            return model.Id;
        }

        public async Task DeleteTask(int taskId)
        {
            await _aixScheduleTaskRepository.DeleteByPkAsync(new AixScheduleTaskInfo { Id = taskId });
        }

        public async Task EnableTask(int taskId)
        {
            await _aixScheduleTaskRepository.UpdateAsync(new AixScheduleTaskInfo { Id = taskId, TaskStatus = 1 });
        }

        public async Task DisableTask(int taskId)
        {
            await _aixScheduleTaskRepository.UpdateAsync(new AixScheduleTaskInfo { Id = taskId, TaskStatus = 0 });
        }
    }
}
