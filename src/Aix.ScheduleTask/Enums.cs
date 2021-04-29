using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask
{
    /// <summary>
    /// 操作状态  0=初始化 /*1=执行中*/ 2=执行成功 9=执行失败  
    /// </summary>
    public enum OPStatus : sbyte
    {
        /// <summary>
        /// 默认
        /// </summary>
        Init = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 2,

        /// <summary>
        /// 失败  
        /// </summary>
        Fail = 9

    }

    /// <summary>
    /// 告警状态：0-默认、-1-锁定状态、1-无需告警、2-告警成功、9-告警失败
    /// </summary>
    public enum AlarmStatus : sbyte
    {
        /// <summary>
        /// 默认
        /// </summary>
        Init = 0,

        /// <summary>
        /// 锁定状态
        /// </summary>
        Lock = -1,

        /// <summary>
        /// 无需告警
        /// </summary>
        NoNeedWarn = 1,

        /// <summary>
        /// 告警成功
        /// </summary>
        WarnSuccess = 2,

        /// <summary>
        /// 告警失败
        /// </summary>
        WarnFail = 9

    }
}
