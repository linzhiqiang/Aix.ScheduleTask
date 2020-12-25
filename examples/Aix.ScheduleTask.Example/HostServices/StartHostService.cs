using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Example.HostServices
{
    public class StartHostService : IHostedService
    {
        private readonly ILogger<StartHostService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IScheduleTaskExecutor _scheduleTaskExecutor;

        public StartHostService(ILogger<StartHostService> logger, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment
            , IScheduleTaskExecutor scheduleTaskExecutor)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hostEnvironment = hostEnvironment;

            #region 定时服务相关

            _scheduleTaskExecutor = scheduleTaskExecutor;
            //注册定时任务事件
            _scheduleTaskExecutor.OnHandleMessage += _scheduleTaskExecutor_OnHandleMessage;

            #endregion

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduleTaskExecutor.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task _scheduleTaskExecutor_OnHandleMessage(Model.ScheduleTaskContext arg)
        {
           //Console.WriteLine($"执行任务:{arg.ExecutorParam}");
            _logger.LogInformation( $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}执行任务:{arg.Id}——{arg.ExecutorParam}");
           // await Task.Delay(TimeSpan.FromSeconds(5));
            await Task.CompletedTask;
        }
    }
}
