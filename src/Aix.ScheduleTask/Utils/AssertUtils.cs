using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.ScheduleTask.Utils
{
    /// <summary>
    /// 业务异常
    /// </summary>
    public class AixScheduleTaskException : Exception
    {
        public int Code { get; set; }

        public AixScheduleTaskException(int code, string message) : base(message)
        {
            this.Code = code;
        }
    }

    internal static class AssertUtils
    {
        public static void IsTrue(bool condition, string errorText)
           => IsTrue(condition, 500, errorText);

        public static void IsTrue(bool condition, int code, string errorText)
        {
            if (!condition)
            {
                throw new AixScheduleTaskException(code, errorText ?? "异常");
            }
        }

        public static void IsNotNull(object obj, string errorText)
        {
            IsTrue(obj != null, errorText);
        }

        public static void IsNotNull(object obj, int code, string errorText)
        {
            IsTrue(obj != null, code, errorText);
        }

        public static void IsNotEmpty(string obj, string errorText)
        {
            IsTrue(!string.IsNullOrEmpty(obj), errorText);
        }
        public static void IsNotEmpty(string obj, int code, string errorText)
        {
            IsTrue(!string.IsNullOrEmpty(obj), code, errorText);
        }

        public static void ThrowException(int code, string errorText)
        {
            IsTrue(false, errorText);
        }
    }
}
