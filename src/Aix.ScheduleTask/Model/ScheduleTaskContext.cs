﻿using System;
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
        public string Executor { get; set; }

        /// <summary>
        /// 执行参数
        /// </summary>
        public string ExecutorParam { get; set; }
    }
}
