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

    }
}
