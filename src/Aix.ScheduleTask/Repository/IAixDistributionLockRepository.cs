using Aix.ScheduleTask.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
    public interface IAixDistributionLockRepository: ICommonRepository
    {
        Task<AixDistributionLock> Get(string lockName);

        Task<string> UseLock(string lockName, int commandTimeout = 300);
    }
}
