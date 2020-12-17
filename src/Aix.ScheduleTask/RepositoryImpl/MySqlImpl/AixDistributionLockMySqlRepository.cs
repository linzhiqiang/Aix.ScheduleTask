using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.RepositoryImpl
{
  public  class AixDistributionLockMySqlRepository : BaseMySqlRepository, IAixDistributionLockRepository
    {

        readonly string AllColumns = " lock_name ";
        public AixDistributionLockMySqlRepository(IServiceProvider provider, AixScheduleTaskOptions options) : base(provider, options.Master)
        {

        }

        public Task<AixDistributionLock> Get(string lockName)
        {
            string sql =$"SELECT {AllColumns} FROM aix_distribution_lock WHERE lock_name = @lockName ";
            return base.GetAsync<AixDistributionLock>(sql,new { lockName });
        }
    }
}
