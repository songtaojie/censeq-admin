using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    public static class EntityConfigurationExtensions
    {
        /// <summary>
        /// 转为snake命名的表名
        /// </summary>
        /// <param name="b"></param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static EntityTypeBuilder<T> ToStarshineTable<T>(this EntityTypeBuilder<T> entityTypeBuilder, string tableName)
            where T : class
        {
            if (tableName.StartsWith(nameof(Volo.Abp.Identity)))
            {
                tableName = tableName[8..];
            }
            else if (tableName.StartsWith(nameof(OpenIddict)))
            {
                tableName = tableName[10..];
            }
            return entityTypeBuilder.ToTable((AbpCommonDbProperties.DbTablePrefix + tableName).ToTableName(), AbpCommonDbProperties.DbSchema);
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
            return CaseHelper.ToSnakeCase(tableName);
        }

       
        /// <summary>
        /// 值对象处理
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="navigationExpression"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static void ValueObject<TEntity, TRelatedEntity>(this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, TRelatedEntity?>> navigationExpression,
            string comment)
            where TEntity : class
        {
            builder.Property(navigationExpression).HasColumnType("jsonb").HasComment(comment).HasConversion<JsonValueConverter<TRelatedEntity>>(new GenericValueComparer<TRelatedEntity>());
        }

        /// <summary>
        /// 值对象处理
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="navigationExpression"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static void ValueObjects<TEntity, TRelatedEntity>(this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, TRelatedEntity[]?>> navigationExpression,
            string comment) where TEntity : class
        {
            builder.Property(navigationExpression).HasColumnType("jsonb").HasComment(comment).HasConversion<JsonValueConverter<TRelatedEntity[]>>(new GenericValueComparer<TRelatedEntity[]>());
        }

        /// <summary>
        /// 值对象处理
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="navigationExpression"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static void ValueObjects<TEntity, TRelatedEntity>(this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, List<TRelatedEntity>?>> navigationExpression,
            string comment) where TEntity : class
        {
            builder.Property(navigationExpression).HasColumnType("jsonb").HasComment(comment).HasConversion<JsonValueConverter<List<TRelatedEntity>>>(new GenericValueComparer<List<TRelatedEntity>>());
        }

        /// <summary>
        /// 值对象处理
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TRelatedEntity"></typeparam>
        /// <param name="builder"></param>
        /// <param name="navigationExpression"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static void ValueObjects<TEntity, TRelatedEntity>(this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, HashSet<TRelatedEntity>?>> navigationExpression,
            string comment) where TEntity : class
        {
            builder.Property(navigationExpression).HasColumnType("jsonb").HasComment(comment).HasConversion<JsonValueConverter<HashSet<TRelatedEntity>>>(new GenericValueComparer<HashSet<TRelatedEntity>>());
        }

    }
}
