using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Foundation.Locked
{
    public class ScheduleTaskDistributedLockEmptyImpl : IScheduleTaskDistributedLock
    {
        public async Task Lock(string key, TimeSpan span, Func<Task> action, Func<Task> concurrentCallback = null)
        {
            await action();
        }
    }
}
