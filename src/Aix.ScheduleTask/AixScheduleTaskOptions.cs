using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask
{
  public  class AixScheduleTaskOptions
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
        ///执行间隔时间（秒） 默认30秒  范围[5,30]
        /// </summary>
        public int CrontabIntervalSecond { get; set; } = 30;
    }
}
