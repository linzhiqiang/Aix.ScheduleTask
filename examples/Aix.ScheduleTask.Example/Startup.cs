using Aix.ScheduleTask.Example.Configs;
using Aix.ScheduleTask.Example.HostServices;
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

            var options = new AixScheduleTaskOptions
            {
                Master = dbOption.Master,
                DBType =1
            };
            services.AddScheduleTask(options);

            #endregion

            //入口服务
            services.AddHostedService<StartHostService>();
        }
    }
}
