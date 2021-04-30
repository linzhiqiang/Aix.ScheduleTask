using Aix.ORM;
using Aix.ORM.Common;
using Aix.ORM.DbTransactionManager;
using Aix.ScheduleTask.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aix.ScheduleTask.Repository
{
    public  interface  IAixScheduleTaskRepository: ICommonRepository
    {
        Task<AixScheduleTaskInfo> GetById(int id);
        Task<PagedList<AixScheduleTaskInfo>> PageQuery(PageView pageView);

        Task<List<AixScheduleTaskInfo>> QueryAllEnabled(long nextExecuteTime);
    }

  
}
