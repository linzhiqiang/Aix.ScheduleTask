using Aix.ORM;
using Aix.ScheduleTask.Foundation;
using Aix.ScheduleTask.Foundation.Locked;
using Aix.ScheduleTask.Repository;
using Aix.ScheduleTask.RepositoryImpl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
            }
            else if (options.DBType == 2)
            {
                services.AddSingleton<IAixScheduleTaskRepository, AixScheduleTaskMySqlRepository>();
                services.AddSingleton<IAixDistributionLockRepository, AixDistributionLockMySqlRepository>();
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
          
            services.AddSingleton<IScheduleTaskExecutor, ScheduleTaskExecutor>();
            services.AddSingleton<IScheduleTaskLifetime,ScheduleTaskLifetime>();
            return services;
        }

        public static IServiceCollection AddScheduleTaskDistributedLock<T>(this IServiceCollection services) where T : class, IScheduleTaskDistributedLock
        {
            services.AddSingleton<IScheduleTaskDistributedLock, T>();
            return services;
        }

        private static void ValidOptions(AixScheduleTaskOptions options)
        {
            if (options == null) throw new Exception("请配置options参数");
            if (string.IsNullOrEmpty(options.Master)) throw new Exception("请配置options.Master");

            if (options.PreReadSecond <= 0) throw new Exception("配置options.CrontabIntervalSecond 非法");
            if (options.PreReadSecond < 5) options.PreReadSecond = 5;
            if (options.PreReadSecond > 30) options.PreReadSecond = 30;
        }
    }


}
