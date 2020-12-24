using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.RepositoryImpl
{
   public class AixDistributionLockSqlServerRepository : BaseSqlServerRepository, IAixDistributionLockRepository
    {
        readonly string AllColumns = " lock_name ";
        public AixDistributionLockSqlServerRepository(IServiceProvider provider, AixScheduleTaskOptions options) : base(provider, options.Master)
        {

        }

        public Task<AixDistributionLock> Get(string lockName)
        {
            string sql = $"SELECT {AllColumns} FROM aix_distribution_lock with(nowait) WHERE lock_name = @lockName ";
            return base.GetAsync<AixDistributionLock>(sql, new { lockName });
        }

        /// <summary>
        /// 开启分布式锁，跟着当前事务结束而结束   sqlserver
        /// </summary>
        /// <param name="lockName">请确保数据库中已存在该lockName</param>
        /// <param name="commandTimeout">超时时间 单位 秒</param>
        /// <returns></returns>
        public Task<string> UseLock(string lockName, int commandTimeout = 300)
        {
            string sql = "select lock_name from aix_distribution_lock  with (rowlock,UpdLock,nowait)  where lock_name=@lockName ";
            return base.ExecuteScalarAsync<string>(sql, new { lockName }, commandTimeout);
        }

    }
}
