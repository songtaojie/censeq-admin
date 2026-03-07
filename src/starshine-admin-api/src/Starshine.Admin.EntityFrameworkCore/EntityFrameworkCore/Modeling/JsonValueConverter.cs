using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    /// <summary>
    /// JSON转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonValueConverter<T>() : ValueConverter<T, string>(v => ToJson(v), v => FromJson(v))
    {
        /// <summary>
        /// JSON序列化配置
        /// </summary>
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            //TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            //{
            //    Modifiers = { JsonExtensions.AppendNonPublicSetters }
            //},
        };

        /// <summary>
        /// 转为JSON
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ToJson(T value)
        {
            if (value is null)
            {
                return default!;
            }

            return JsonSerializer.Serialize(value, value.GetType(), JsonSerializerOptions);
        }

        /// <summary>
        /// 转为对象
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static T FromJson(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return JsonSerializer.Deserialize<T>(s, JsonSerializerOptions)!;
            }

            return default!;
        }
    }
}
