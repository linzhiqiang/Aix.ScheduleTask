using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask
{
    public class ScheduleTaskDTO
    {
        /// <summary>
        /// 修改时有用
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 任务分组或者标识 自己根据使用情况进行区分 
        /// </summary>
        public string TaskGroup { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 定时表达式  
        ///  sec(0 - 59)
        ///  min(0 - 59)
        /// hour(0 - 23)
        /// hour(0 - 23
        ///  day of month(1 - 31)
        ///  month(1 - 12)
        /// day of week(0 - 6) (Sunday=0)
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string TaskContent { get; set; }

        /// <summary>
        /// 最大重试次数 0=不重试 
        /// </summary>
        public int MaxRetryCount { get; set; }

        public string UserId { get; set; }
    }
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
