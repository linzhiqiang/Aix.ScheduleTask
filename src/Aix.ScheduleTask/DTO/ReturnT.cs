using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aix.ScheduleTask.DTO
{
    [DataContract]
    public class ReturnT
    {
        public const int SUCCESS_CODE = 2;
        public const int FAIL_CODE = 9;
        public int Code { get; set; }
        public string Message { get; set; }

        private ReturnT() { }

        private ReturnT(int code, string message)
        {
            Code = code;
            Message = message;
        }


        public static ReturnT Failed(string msg)
        {
            return new ReturnT(FAIL_CODE, msg);
        }
        public static ReturnT Success(string msg = "success")
        {
            return new ReturnT(SUCCESS_CODE, msg);
        }
    }

}
