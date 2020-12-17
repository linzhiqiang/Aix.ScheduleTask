using Aix.ORM.Common;
using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.RepositoryImpl
{
    public class AixScheduleTaskMySqlRepository : BaseMySqlRepository,IAixScheduleTaskRepository
    {
        public AixScheduleTaskMySqlRepository(IServiceProvider provider, AixScheduleTaskOptions options) : base(provider, options.Master)
        {

        }

        public  async Task<PagedList<AixScheduleTaskInfo>> PageQuery(PageView pageView)
        {
            var column = GetAllColumns<AixScheduleTaskInfo>();
            var table = "aix_schedule_task_info";

            var sqlCondition = new StringBuilder();
            sqlCondition.Append(" AND status=1 ");
            string sqlOrder = " ORDER BY  id  ASC ";

            return await base.PagedQueryAsync<AixScheduleTaskInfo>(pageView, column, table, sqlCondition.ToString(), null, sqlOrder);
        }

        public  Task<List<AixScheduleTaskInfo>> QueryAllEnabled(long nextExecuteTime)
        {
            var column = GetAllColumns<AixScheduleTaskInfo>();
            var sql = $"SELECT {column} FROM aix_schedule_task_info WHERE status=1 AND next_execute_time<=@nextExecuteTime ORDER BY id  ";
            return base.QueryAsync<AixScheduleTaskInfo>(sql, new { nextExecuteTime });
        }

        
    }
}
