using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Foundation
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public interface IScheduleTaskDistributedLock
    {
        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="key">分布式锁键</param>
        /// <param name="span">超过该时间 自动释放</param>
        /// <param name="action">同步代码块</param>
        /// <param name="concurrentCallback">存在并发时的回调，默认是抛出异常，可以重写</param>
        /// <returns></returns>
        Task Lock(string key, TimeSpan span, Func<Task> action, Func<Task> concurrentCallback = null);

    }
}
