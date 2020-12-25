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
        ///预提前读取时间（秒） 默认10秒  范围[5,30]
        /// </summary>
        public int PreReadSecond { get; set; } = 10;

        /// <summary>
        /// 集群类型 0=多集群（默认值）  1=单实例
        /// </summary>
        public int ClusterType { get; set; } = 0;
    }
}
