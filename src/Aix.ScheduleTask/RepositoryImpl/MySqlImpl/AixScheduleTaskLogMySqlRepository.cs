using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask.RepositoryImpl
{
  public  class AixScheduleTaskLogMySqlRepository : BaseMySqlRepository, IAixScheduleTaskLogRepository
    {
        public AixScheduleTaskLogMySqlRepository(IServiceProvider provider, AixScheduleTaskOptions options) : base(provider, options.Master)
        {

        }
    }
}
