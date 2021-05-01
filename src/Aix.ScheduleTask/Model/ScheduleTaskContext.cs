using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask
{
    public class ScheduleTaskContext
    {
        /// <summary>
        /// 定时任务logid
        /// </summary>
        public int LogId { get; set; }

        public int TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 执行器 根据需要扩展
        /// </summary>
        public string TaskGroup { get; set; }

        /// <summary>
        /// 任务内容
        /// </summary>
        public string TaskContent { get; set; }
    }

    /// <summary>
    /// 执行结果信息
    /// </summary>
    public class ExecuteResult
    {
        /// <summary>
        /// 定时任务logid
        /// </summary>
        public int LogId { get; set; }

        /// <summary>
        /// 状态码  0=执行成功 非0=执行失败  
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 直接结果描述
        /// </summary>
        public string Message { get; set; }
    }
}
