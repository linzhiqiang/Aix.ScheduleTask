using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask.Utils
{
    internal static class StringUtils
    {
        /// <summary>
        /// 找到一个不为empty的返回
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string IfEmpty(params string[] strs)
        {
            string result = string.Empty;
            if (strs != null)
            {
                foreach (var item in strs)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        result = item;
                        break;
                    }
                }
            }

            return result;
        }

        public static string SubString(string value, int length)
        {
            if (value == null) return string.Empty;
            if (value.Length <= length) return value;
            return value.Substring(0, length);
        }

    }
}
