using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask
{
    /// <summary>
    /// 定时任务接口
    /// </summary>
    public interface IScheduleTaskService : IDisposable
    {
        /// <summary>
        /// 开始定时任务服务
        /// </summary>
        /// <returns></returns>
        Task Start();

        /// <summary>
        /// 接收定时任务事件
        /// </summary>
        event Func<ScheduleTaskContext, Task> OnHandleMessage;

        /// <summary>
        /// 保存执行结果
        /// </summary>
        /// <param name="resultDTO"></param>
        /// <returns></returns>
        Task SaveExecuteResult(ExecuteResultDTO resultDTO);
    }
}
