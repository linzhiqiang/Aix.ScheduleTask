using Aix.ORM;
using Aix.ORM.DbTransactionManager;
using Aix.ORM.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
    /// <summary>
    /// 公共接口
    /// </summary>
    public interface ICommonRepository : IRepository
    {
       // Task<string> UseLock(string lockName, int commandTimeout = 300);
    }
}
