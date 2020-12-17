using Aix.ORM;
using Aix.ORM.DbTransactionManager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
    /// <summary>
    /// 公共接口
    /// </summary>
    public interface IRepository
    {
        IDBTransScope BeginTransScope(TransScopeOption scopeOption = TransScopeOption.Required);

        void OpenNewContext();

        Task<string> UseLock(string lockName, int commandTimeout = 300);

        Task<long> InsertAsync(BaseEntity entity);

        Task<int> BatchInsertAsync<T>(List<T> list) where T : BaseEntity;
        Task<int> UpdateAsync(BaseEntity model);

        Task<int> BatchUpdateAsync<T>(List<T> list) where T : BaseEntity;
    }
}
