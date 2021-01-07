using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask.Model
{
    public class ScheduleTaskContext
    {
        /// <summary>
        /// 定时任务id
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
}
