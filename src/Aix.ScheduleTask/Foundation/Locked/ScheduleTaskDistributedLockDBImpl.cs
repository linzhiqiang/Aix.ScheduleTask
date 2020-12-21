using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Foundation.Locked
{
    public class ScheduleTaskDistributedLockDBImpl : IScheduleTaskDistributedLock
    {
        private readonly IAixDistributionLockRepository _aixDistributionLockRepository;
        public ScheduleTaskDistributedLockDBImpl(IAixDistributionLockRepository aixDistributionLockRepository)
        {
            _aixDistributionLockRepository = aixDistributionLockRepository;
        }
        public async Task Lock(string key, TimeSpan span, Func<Task> action, Func<Task> concurrentCallback = null)
        {
            using (var scope = _aixDistributionLockRepository.BeginTransScope())
            {
                await _aixDistributionLockRepository.UseLock(key, (int)span.TotalSeconds);

                await action();

                scope.Commit();
            }
        }
      
    }
}
