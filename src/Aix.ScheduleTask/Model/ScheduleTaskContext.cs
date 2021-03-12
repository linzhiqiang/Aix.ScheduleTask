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
        public int Id { get; set; }

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
    public class ExecuteResultDTO
    {
        /// <summary>
        /// 定时任务logid
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 状态码 
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 直接结果描述
        /// </summary>
        public string Message { get; set; }
    }
}
