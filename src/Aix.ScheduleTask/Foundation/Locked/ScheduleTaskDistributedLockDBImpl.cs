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
        private readonly IScheduleTaskLifetime _scheduleTaskLifetime;
        public ScheduleTaskDistributedLockDBImpl(IAixDistributionLockRepository aixDistributionLockRepository, IScheduleTaskLifetime scheduleTaskLifetime)
        {
            _aixDistributionLockRepository = aixDistributionLockRepository;
            _scheduleTaskLifetime = scheduleTaskLifetime;
        }

        public async Task Lock(string key, TimeSpan span, Func<Task> action, Func<Task> concurrentCallback = null)
        {
            using (var scope = _aixDistributionLockRepository.BeginTransScope())
            {
                try
                {
                    var task = _aixDistributionLockRepository.UseLock(key, (int)span.TotalSeconds);
                    await task.TimeoutAfter(TimeSpan.FromSeconds(2), _scheduleTaskLifetime.ScheduleTaskStopping);
                }
                //catch (TimeoutException)
                catch (Exception)
                {
                    if (concurrentCallback != null)
                    {
                        await concurrentCallback();
                    }
                    else
                    {
                        throw;
                    }
                }
                await action();

                scope.Commit();
            }
        }
        public async Task LockPld(string key, TimeSpan span, Func<Task> action, Func<Task> concurrentCallback = null)
        {
            using (var scope = _aixDistributionLockRepository.BeginTransScope())
            {
                try
                {
                    await _aixDistributionLockRepository.UseLock(key, (int)span.TotalSeconds);
                }
                catch (Exception ex)
                {
                    var lockTimeout = false;
                    //sqlserver  ErrorCode - 2146232060  message=已超过了锁请求超时时段。  
                    if (ex.GetType().GetProperty("ErrorCode")?.GetValue(ex)?.ToString() == "-2146232060")
                    {
                        lockTimeout = true;
                    }
                    else if (1 == 1) //mysql的错误码判断
                    {
                        lockTimeout = true;
                    }
                    if (lockTimeout == true)
                    {
                        if (concurrentCallback != null)
                        {
                            await concurrentCallback();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                await action();

                scope.Commit();
            }
        }



    }
}
