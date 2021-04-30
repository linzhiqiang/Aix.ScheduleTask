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
        private readonly IScheduleTaskService _scheduleTaskService;

        public StartHostService(ILogger<StartHostService> logger, IServiceProvider serviceProvider, IHostEnvironment hostEnvironment
            , IScheduleTaskService scheduleTaskService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hostEnvironment = hostEnvironment;

            #region 定时服务相关

            _scheduleTaskService = scheduleTaskService;
            //注册定时任务事件
            _scheduleTaskService.OnHandleMessage += _scheduleTaskExecutor_OnHandleMessage;

            #endregion

        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduleTaskService.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task _scheduleTaskExecutor_OnHandleMessage(ScheduleTaskContext arg)
        {
            int code = 0;
            string message = "success";
            try
            {
                _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}执行任务:{arg.LogId}——{arg.TaskContent}");
                // await Task.Delay(TimeSpan.FromSeconds(5));
                throw new Exception("测试重试");
            }
            //catch(BizException ex)
            //{
            //    code = ex.Code;
            //    message = ex.Message;
            //}
            catch (Exception ex)
            {
                code = -1;
                message = ex.Message;
            }
            finally
            {
                await _scheduleTaskService.SaveExecuteResult(new ExecuteResultDTO { LogId = arg.LogId, Code = code, Message = message });
            }
        }
    }
}
