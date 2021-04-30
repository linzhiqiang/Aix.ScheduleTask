using Aix.ScheduleTask.Model;
using Aix.ScheduleTask.Utils;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask
{
  public  class CrontabHelper
    {
        private static ConcurrentDictionary<string, CrontabSchedule> CrontabScheduleCache = new ConcurrentDictionary<string, CrontabSchedule>();

        public static CrontabSchedule ParseCron(ILogger logger,AixScheduleTaskInfo task)
        {
            string cron = task.Cron;
            CrontabSchedule result = null;
            try
            {
                if (string.IsNullOrEmpty(cron)) throw new Exception($"Aix.ScheduleTask任务{task.Id},{task.TaskName}表达式配置为空");
                if (CrontabScheduleCache.TryGetValue(cron, out result))
                {
                    return result;
                }
                var options = new CrontabSchedule.ParseOptions
                {
                    IncludingSeconds = cron.Split(' ').Length > 5,
                };
                result = CrontabSchedule.Parse(cron, options);
                CrontabScheduleCache.TryAdd(cron, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Aix.ScheduleTask定时任务解析出错 {task.Id},{task.TaskName},{task.Cron}");
            }
            return result;
        }

        public static TimeSpan GetNextDueTime(CrontabSchedule Schedule, DateTime LastDueTime, DateTime now)
        {
            var nextOccurrence = GetNexeTime(Schedule, LastDueTime);
            TimeSpan dueTime = nextOccurrence - now;// DateTime.Now;

            //if (dueTime.TotalMilliseconds <= 0)
            //{
            //    dueTime = TimeSpan.Zero;
            //}

            return dueTime;
        }

        private static DateTime GetNexeTime(CrontabSchedule Schedule, DateTime LastDueTime)
        {
            return Schedule.GetNextOccurrence(LastDueTime);
        }

        /// <summary>
        /// 时间戳转时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private static DateTime TimeStampToDateTime(long timestamp)
        {
            return DateTimeUtils.TimeStampToDateTime(timestamp);
        }
    }
}
