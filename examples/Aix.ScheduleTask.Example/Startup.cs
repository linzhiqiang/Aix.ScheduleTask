using Aix.ScheduleTask.Example.Configs;
using Aix.ScheduleTask.Example.HostServices;
using Aix.ScheduleTask.Foundation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Example
{
    public class Startup
    {
        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var dbOption = context.Configuration.GetSection("connectionStrings").Get<DBOptions>();

            #region 定时服务相关

            services.AddScheduleTask(options =>
            {
                options.Master = dbOption.Master;
                options.DBType = 1;
                options.ConsumerThreadCount = 2;
                // options.ClusterType = 1;
                options.SaveExecuteLog = false;
            })
            //.AddScheduleTaskDistributedLock<ScheduleTaskDistributedLockRedisImpl>()  //默认是用数据库的锁实现的，也可以替换为别的如redis锁等
            ;

            #endregion

            //入口服务
            services.AddHostedService<StartHostService>();
        }
    }

    public class ScheduleTaskDistributedLockRedisImpl : IScheduleTaskDistributedLock
    {
        public async Task Lock(string key, TimeSpan span, Func<Task> action, Func<Task> concurrentCallback = null)
        {
            await action();
        }
    }
}
