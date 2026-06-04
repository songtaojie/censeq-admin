using System.Linq;
using Censeq.AuditLogging.Entities;
using Microsoft.EntityFrameworkCore;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志 EF Core 查询扩展方法。
/// </summary>
public static class CenseqAuditLoggingEfCoreQueryableExtensions
{
    /// <summary>
    /// 包含审计日志详情。
    /// </summary>
    /// <param name="queryable">queryable。</param>
    /// <param name="include">include。</param>
    /// <returns>包含详情的查询。</returns>
    public static IQueryable<AuditLog> IncludeDetails(
        this IQueryable<AuditLog> queryable,
        bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.Actions)
            .Include(x => x.EntityChanges).ThenInclude(ec => ec.PropertyChanges);
    }

    /// <summary>
    /// 包含审计日志详情。
    /// </summary>
    /// <param name="queryable">queryable。</param>
    /// <param name="include">include。</param>
    /// <returns>包含详情的查询。</returns>
    public static IQueryable<EntityChange> IncludeDetails(
        this IQueryable<EntityChange> queryable,
        bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable.Include(x => x.PropertyChanges);
    }
}
