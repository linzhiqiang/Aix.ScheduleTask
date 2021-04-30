using Aix.ScheduleTask.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
  public  interface IAixScheduleTaskLogRepository : ICommonRepository
    {
        Task<AixScheduleTaskLog> GetById(int id);
        Task<int> Delete(DateTime expiration);

        Task<List<int>> QueryFailJobLogIds(int count = 1000);

        Task<int> UpdateAlarmStatus(int id, int oldAlarmStatus, int newAlarmStatus);
    }
}
