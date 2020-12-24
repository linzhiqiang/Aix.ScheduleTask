using Aix.ORM;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Aix.ScheduleTask.Utils;

namespace Aix.ScheduleTask.RepositoryImpl
{
    /// <summary>
    /// sql执行 跟踪
    /// </summary>
    public class SqlExecuteTrace : AbstractSqlExecuteTrace
    {
        protected IServiceProvider _provider;
        private ILogger<SqlExecuteTrace> _logger;
        private Stopwatch _stopwatch;

        public SqlExecuteTrace(string sql, object paras, IServiceProvider provider) : base(sql, paras)
        {
            _provider = provider;
            _logger = provider.GetService<ILogger<SqlExecuteTrace>>();
        }
        public override void ExecuteStart()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public override void ExecuteException(Exception ex)
        {
           // _logger.LogError(ex, "SQL Error,  SQL={0},params = {1}", Sql, JsonUtils.ToJson(Param));
        }
        public override void ExecuteEnd()
        {
            _stopwatch.Stop();

            var totalTime = _stopwatch.ElapsedMilliseconds;

            if (totalTime > 500)
            {
                _logger.LogWarning("SQL Warning:total time in  {0} ms,SQL={1},params = {2}", totalTime, Sql, JsonUtils.ToJson(Param));
            }
            else
            {
                _logger.LogDebug("SQL Debug:total time in {0} ms,SQL={1},params = {2}", totalTime, Sql, JsonUtils.ToJson(Param));
            }
        }



    }
}
