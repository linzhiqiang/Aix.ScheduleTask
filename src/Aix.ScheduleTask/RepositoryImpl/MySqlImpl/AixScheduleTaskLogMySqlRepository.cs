using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.RepositoryImpl
{
  public  class AixScheduleTaskLogMySqlRepository : BaseMySqlRepository, IAixScheduleTaskLogRepository
    {
        public AixScheduleTaskLogMySqlRepository(IServiceProvider provider, AixScheduleTaskOptions options) : base(provider, options.Master)
        {

        }

        public Task<int> Delete(DateTime expiration)
        {
            string sql = "DELETE FROM aix_schedule_task_log WHERE create_time <=@expiration ";
            return ExcuteAsync(sql,new { expiration});
        }
    }
}
