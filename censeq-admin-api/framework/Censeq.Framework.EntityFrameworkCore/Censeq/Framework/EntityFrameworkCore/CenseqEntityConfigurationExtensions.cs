using Censeq.Framework.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Censeq.Framework.EntityFrameworkCore
{
    public static class CenseqEntityConfigurationExtensions
    {

        /// <summary>
        /// 转为snake命名的表名
        /// </summary>
        /// <param name="b"></param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static EntityTypeBuilder<T> ToCenseqTable<T>(this EntityTypeBuilder<T> entityTypeBuilder, string tableName)
            where T : class
        {
           
            return entityTypeBuilder.ToTable((CenseqCommonDbProperties.DbTablePrefix + tableName).ToTableName(), CenseqCommonDbProperties.DbSchema);
        }

        /// <summary>
        /// 转为snake命名的表名
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        public static string ToTableName(this string s, string? prefix = null)
        {
            string tableName;
            if (prefix == null)
            {
                tableName = s;
            }
            else
            {
                if (prefix.EndsWith('_'))
                {
                    tableName = $"{prefix}{s}";
                }
                else
                {
                    tableName = $"{prefix}_{s}";
                }
            }
            return tableName.ToSnakeCase();
        }
    }
}
