/*
该文件为自动生成，不要修改。
生成时间：2021-03-12 14:42:28。
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aix.ORM;

namespace Aix.ScheduleTask.Model
{
    /// <summary>
    /// 定时任务log
    /// <summary>
    [Table("aix_schedule_task_log")]
    public partial class AixScheduleTaskLog : BaseEntity
    {
        private int _id; 
        private int _schedule_task_id; 
        private int _result_code; 
        private string _result_message; 
        private int _status; 
        private DateTime _create_time; 
        private DateTime _modify_time; 

        /// <summary>
        /// 主键  int(4)
        /// <summary>
        [Column("id",IsNullable=false)]
        [PrimaryKey]
        [Identity]
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("id"); }
        }
        /// <summary>
        /// 定时任务id  int(4)
        /// <summary>
        [Column("schedule_task_id",IsNullable=false)]
        public int ScheduleTaskId
        {
            get { return _schedule_task_id; }
            set { _schedule_task_id = value; OnPropertyChanged("schedule_task_id"); }
        }
        /// <summary>
        /// 结果code  int(4)
        /// <summary>
        [Column("result_code",IsNullable=false)]
        public int ResultCode
        {
            get { return _result_code; }
            set { _result_code = value; OnPropertyChanged("result_code"); }
        }
        /// <summary>
        /// 结果信息  varchar(500)
        /// <summary>
        [Column("result_message",IsNullable=true)]
        public string ResultMessage
        {
            get { return _result_message; }
            set { _result_message = value; OnPropertyChanged("result_message"); }
        }
        /// <summary>
        /// 状态 0=初始化 1=执行中 2=执行成功 9=执行失败  int(4)
        /// <summary>
        [Column("status",IsNullable=false,DefaultValue="0")]
        public int Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("status"); }
        }
        /// <summary>
        /// 创建日期  datetime(8)
        /// <summary>
        [Column("create_time",IsNullable=false,DefaultValue="getdate()")]
        public DateTime CreateTime
        {
            get { return _create_time; }
            set { _create_time = value; OnPropertyChanged("create_time"); }
        }
        /// <summary>
        /// 修改日期  datetime(8)
        /// <summary>
        [Column("modify_time",IsNullable=false,DefaultValue="getdate()")]
        public DateTime ModifyTime
        {
            get { return _modify_time; }
            set { _modify_time = value; OnPropertyChanged("modify_time"); }
        }
    }

}