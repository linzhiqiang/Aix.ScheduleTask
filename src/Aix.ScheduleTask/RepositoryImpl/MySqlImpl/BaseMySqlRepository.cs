using Aix.ORM;
using Aix.ORM.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.RepositoryImpl
{
    public abstract class BaseMySqlRepository : MySqlRepository
    {

        protected IServiceProvider _provider;
        public BaseMySqlRepository(IServiceProvider provider, string connectionStrings) : base(connectionStrings)
        {
            _provider = provider;
        }

        protected override AbstractSqlExecuteTrace GetSqlExecuteTrace(string sql, object paras)
        {
            return new SqlExecuteTrace(sql, paras, _provider);
        }

        /// <summary>
        /// 开启分布式锁，跟着当前事务结束而结束 mysql
        /// </summary>
        /// <param name="lockName">请确保数据库中已存在该lockName</param>
        /// <param name="commandTimeout">超时时间 单位 秒</param>
        /// <returns></returns>
        public Task<string> UseLock(string lockName, int commandTimeout = 300)
        {
            string sql = "select lock_name from aix_distribution_lock where lock_name=@lockName for update";
            return base.ExecuteScalarAsync<string>(sql, new { lockName }, commandTimeout);
        }

    }
}
