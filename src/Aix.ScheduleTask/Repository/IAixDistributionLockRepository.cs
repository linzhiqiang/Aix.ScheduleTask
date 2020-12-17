using Aix.ScheduleTask.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
    public interface IAixDistributionLockRepository:IRepository
    {
        Task<AixDistributionLock> Get(string lockName);
    }
}
