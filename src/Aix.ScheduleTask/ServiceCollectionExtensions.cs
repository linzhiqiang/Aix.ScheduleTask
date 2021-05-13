using Aix.ORM;
using Aix.ScheduleTask.Executors;
using Aix.ScheduleTask.Foundation;
using Aix.ScheduleTask.Foundation.Locked;
using Aix.ScheduleTask.Repository;
using Aix.ScheduleTask.RepositoryImpl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduleTask(this IServiceCollection services, Action<AixScheduleTaskOptions> setupAction)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            Dapper.SqlMapper.Settings.CommandTimeout = 30;//秒
            //ORMSettings.SetConnectionFactory(new DBConnectionFactory());

            var options = new AixScheduleTaskOptions();
            if (setupAction != null)
            {
                setupAction(options);
            }
            ValidOptions(options);
            services.AddSingleton(options);
            if (options.DBType == 1)
            {
                services.AddSingleton<IAixScheduleTaskRepository, AixScheduleTaskSqlServerRepository>();
                services.AddSingleton<IAixDistributionLockRepository, AixDistributionLockSqlServerRepository>();
                services.AddSingleton<IAixScheduleTaskLogRepository, AixScheduleTaskLogSqlServerRepository>();
            }
            else if (options.DBType == 2)
            {
                services.AddSingleton<IAixScheduleTaskRepository, AixScheduleTaskMySqlRepository>();
                services.AddSingleton<IAixDistributionLockRepository, AixDistributionLockMySqlRepository>();
                services.AddSingleton<IAixScheduleTaskLogRepository, AixScheduleTaskLogMySqlRepository>();
            }
            else
            {
                throw new Exception("请配置DBType，1=SqlServer（默认值） 2=Mysql ");
            }

            if (options.ClusterType == 1)
            {
                services.AddSingleton<IScheduleTaskDistributedLock, ScheduleTaskDistributedLockEmptyImpl>();
            }
            else
            {
                services.AddSingleton<IScheduleTaskDistributedLock, ScheduleTaskDistributedLockDBImpl>();
            }

            services.AddSingleton<IScheduleTaskService, ScheduleTaskService>();
            services.AddSingleton<IScheduleTaskAdminService, ScheduleTaskAdminService>();
            services.AddSingleton<IScheduleTaskLifetime, ScheduleTaskLifetime>();
            services.AddSingleton<ScheduleTaskExecutor>();
            services.AddSingleton<ExpireLogExecutor>();
            services.AddSingleton<ErrorTaskExecutor>();

            services.AddAddMultithreadExecutor(options.ConsumerThreadCount);
            return services;
        }

        /// <summary>
        /// 添加自己实现的分布式锁
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScheduleTaskDistributedLock<T>(this IServiceCollection services) where T : class, IScheduleTaskDistributedLock
        {
            services.AddSingleton<IScheduleTaskDistributedLock, T>();
            return services;
        }


        private static void AddAddMultithreadExecutor(this IServiceCollection services, int consumerThreadCount)
        {
            if (consumerThreadCount <= 0) throw new ArgumentException("Aix.ScheduleTask消费者线程数必须大于0");
            services.AddSingleton(serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<MyMultithreadTaskExecutor>>();
                var taskExecutor = new MyMultithreadTaskExecutor(options =>
                {
                    options.ThreadCount = consumerThreadCount;// Environment.ProcessorCount * 2;
                });
                taskExecutor.OnException += ex =>
                {
                    logger.LogError(ex, "Aix.ScheduleTask本地多线程任务执行器执行出错");
                    return Task.CompletedTask;
                };
                taskExecutor.Start();
                logger.LogInformation($"Aix.ScheduleTask本地多线程任务执行器开始 ThreadCount={taskExecutor.ThreadCount}......");
                return taskExecutor;
            });
        }

        private static void ValidOptions(AixScheduleTaskOptions options)
        {
            if (options == null) throw new Exception("请配置options参数");
            if (string.IsNullOrEmpty(options.Master)) throw new Exception("请配置options.Master");

            if (options.PreReadSecond <= 0) throw new Exception("配置options.PreReadSecond 非法");
            if (options.PreReadSecond < 5) options.PreReadSecond = 5;
            if (options.PreReadSecond > 30) options.PreReadSecond = 30;
            if (options.RetryIntervalMillisecond < 10) options.RetryIntervalMillisecond = 10;
        }
    }


}
