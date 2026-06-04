using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict查询扩展方法。
/// </summary>
public static class CenseqOpenIddictQueryableExtensions
{
    /// <summary>
    /// 满足条件时跳过。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="condition">condition。</param>
    /// <param name="count">count。</param>
    /// <returns>操作结果。</returns>
    public static TQueryable SkipIf<T, TQueryable>([NotNull] this TQueryable query, bool condition, int? count)
        where TQueryable : IQueryable<T>
    {
        Check.NotNull(query, nameof(query));

        if (condition && count.HasValue)
        {
            return (TQueryable)query.Skip(count.Value);
        }

        return query;
    }

    /// <summary>
    /// 满足条件时取值。
    /// </summary>
    /// <param name="query">查询。</param>
    /// <param name="condition">condition。</param>
    /// <param name="count">count。</param>
    /// <returns>操作结果。</returns>
    public static TQueryable TakeIf<T, TQueryable>([NotNull] this TQueryable query, bool condition, int? count)
        where TQueryable : IQueryable<T>
    {
        Check.NotNull(query, nameof(query));

        if (condition && count.HasValue)
        {
            return (TQueryable)query.Take(count.Value);
        }

        return query;
    }
}
