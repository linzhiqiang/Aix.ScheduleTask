using Aix.ORM;
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
        public static IServiceCollection AddScheduleTask(this IServiceCollection services, AixScheduleTaskOptions options)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            Dapper.SqlMapper.Settings.CommandTimeout = 30;//秒
            //ORMSettings.SetConnectionFactory(new DBConnectionFactory());

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

            services.AddSingleton<IScheduleTaskExecutor, ScheduleTaskExecutor>();
            return services;
        }

        private static void ValidOptions(AixScheduleTaskOptions options)
        {
            if (options == null) throw new Exception("请配置options参数");
            if (string.IsNullOrEmpty(options.Master)) throw new Exception("请配置options.Master");

            if (options.CrontabIntervalSecond <= 0) throw new Exception("配置options.CrontabIntervalSecond 非法");
            if (options.CrontabIntervalSecond < 5) options.CrontabIntervalSecond = 5;
            if (options.CrontabIntervalSecond > 30) options.CrontabIntervalSecond = 30;
        }
    }


}
