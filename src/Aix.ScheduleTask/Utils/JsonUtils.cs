using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace Aix.ScheduleTask.Utils
{
    internal static class JsonUtils
    {
        static JsonSerializerOptions Options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            //WriteIndented=true //格式化的
        };

        public static string ToJson(object obj)
        {
            if (obj == null) return string.Empty;
            if (obj is string || obj.GetType().IsValueType)
            {
                return obj.ToString();
            }
            return System.Text.Json.JsonSerializer.Serialize(obj,Options);
        }
        public static T FromJson<T>(string str)
        {
            var result = FromJson(str, typeof(T));
            if (result == null) return default(T);
            return (T)result;
        }

        public static object FromJson(string str, Type type)
        {
            if (string.IsNullOrWhiteSpace(str) || type == null)
            {
                return null;
            }

            if (type == typeof(string))
            {
                return str;
            }
            if (type.IsValueType)
            {
                return Convert.ChangeType(str, type);
            }

            return System.Text.Json.JsonSerializer.Deserialize(str, type, Options);
        }

        public static void Serialize(object data, Stream outputStream)
        {
            var writer = new Utf8JsonWriter(outputStream);
            JsonSerializer.Serialize(writer, data, data.GetType(), Options);
            writer.Flush();
        }

        public static object Deserialize(Stream inputStream, Type objectType)
        {
            using (var reader = new StreamReader(inputStream))
            {
                return JsonSerializer.Deserialize(reader.ReadToEnd(), objectType, Options);
            }
               
        }
    }
}
