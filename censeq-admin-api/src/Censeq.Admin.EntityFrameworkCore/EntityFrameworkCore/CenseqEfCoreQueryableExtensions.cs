using Microsoft.EntityFrameworkCore;
using Censeq.Admin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Censeq.Admin.EntityFrameworkCore;

public static class StarshineEfCoreQueryableExtensions
{
    /// <summary>
    /// 包含详情
    /// </summary>
    /// <param name="queryable"></param>
    /// <param name="include"></param>
    /// <returns></returns>
    public static IQueryable<Tenant> IncludeDetails(this IQueryable<Tenant> queryable, bool include = true)
    {
        if (!include) return queryable;
        return queryable.Include(x => x.ConnectionStrings);
    }

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
            .Include(x => x.EntityChanges)
            .ThenInclude(ec => ec.PropertyChanges);
    }

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
