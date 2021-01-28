using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask.RepositoryImpl
{
   public class AixScheduleTaskLogSqlServerRepository: BaseSqlServerRepository, IAixScheduleTaskLogRepository
    {
        public AixScheduleTaskLogSqlServerRepository(IServiceProvider provider, AixScheduleTaskOptions options) : base(provider, options.Master)
        {

        }
    }
}
