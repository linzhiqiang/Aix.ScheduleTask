using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
  public  interface IAixScheduleTaskLogRepository : ICommonRepository
    {
        Task<int> Delete(DateTime expiration);
    }
}
