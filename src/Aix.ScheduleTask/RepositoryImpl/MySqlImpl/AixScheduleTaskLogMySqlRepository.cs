using Aix.ScheduleTask.Model;
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

        public Task<AixScheduleTaskLog> GetById(int id)
        {
            var allColumns = GetAllColumns<AixScheduleTaskLog>();
            var sql = $"SELECT {allColumns} FROM aix_schedule_task_log WHERE id=@id ";
            return GetAsync<AixScheduleTaskLog>(sql, new { id });
        }

        public Task<int> Delete(DateTime expiration)
        {
            string sql = "DELETE FROM aix_schedule_task_log WHERE create_time <=@expiration ";
            return ExcuteAsync(sql,new { expiration});
        }

        public Task<List<int>> QueryFailJobLogIds(int count = 1000)
        {
            var sql = $"SELECT  id FROM aix_schedule_task_log WHERE  alarm_status=0 AND (trigger_code=9 OR result_code=9) LIMIT {count}";
            return QueryAsync<int>(sql, new { });
        }

        public Task<int> UpdateAlarmStatus(int id, int oldAlarmStatus, int newAlarmStatus)
        {
            var sql = "UPDATE aix_schedule_task_log SET  alarm_status=@newAlarmStatus WHERE id=@id AND alarm_status=@oldAlarmStatus ";
            return base.ExcuteAsync(sql, new { id, newAlarmStatus, oldAlarmStatus });
        }
    }
}
