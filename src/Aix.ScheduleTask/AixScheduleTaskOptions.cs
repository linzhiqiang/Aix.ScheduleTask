﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask
{
    public class AixScheduleTaskOptions
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string Master { get; set; }

        /// <summary>
        /// 数据库类型   1=SqlServer（默认值） 2=Mysql 
        /// </summary>
        public int DBType { get; set; } = 1;

        /// <summary>
        ///预提前读取时间（秒） 默认10秒  范围[5,30]
        /// </summary>
        public int PreReadSecond { get; set; } = 10;

        /// <summary>
        /// 集群类型 0=多集群（默认值）  1=单实例
        /// </summary>
        public int ClusterType { get; set; } = 0;

        /// <summary>
        /// 消费线程数 默认2
        /// </summary>
        public int ConsumerThreadCount { get; set; } = 2;

        /// <summary>
        /// 保存日志信息最大长度 日志表的result_message最大长度 默认500
        /// </summary>
        public int LogResultMessageMaxLength { get; set; } = 500;

        /// <summary>
        /// 积压的任务数,来不及执行的任务数超过该值，就打印报警日志并暂停5秒
        /// </summary>
        public int Backlog { get; set; } = 10000;

        /// <summary>
        /// 任务数据有效期 默认7天=168 单位  小时
        /// </summary>
        public int LogExpireHour { get; set; } = 168;

        /// <summary>
        /// 错误任务 重试间隔时间 默认10秒
        /// </summary>
        public int RetryIntervalMillisecond { get; set; } = 10;

    }
}
