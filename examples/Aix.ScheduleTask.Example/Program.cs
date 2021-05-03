using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace Aix.ScheduleTask.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(200, 200);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
               .ConfigureHostConfiguration(configurationBuilder =>
               {
               })
              .ConfigureAppConfiguration((hostBulderContext, configurationBuilder) =>
              {
                  //配置环境变量 ASPNETCORE _ENVIRONMENT: Development/Staging/Production(默认值) 
                  //以下加载配置文件的方式，是系统的默认行为，如果改变配置文件路径 需要自己加载，否则没必要了
                  /*
                   var environmentName = hostBulderContext.HostingEnvironment.EnvironmentName;
                   configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                   configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);// 覆盖前面的相同内容
                 */
              })
               .ConfigureLogging((hostBulderContext, loggingBuilder) =>
               {

               })
               .ConfigureServices(Startup.ConfigureServices);
        }
    }
}
