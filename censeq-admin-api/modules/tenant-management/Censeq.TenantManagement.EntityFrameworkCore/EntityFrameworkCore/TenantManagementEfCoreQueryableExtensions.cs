using Censeq.TenantManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace Censeq.TenantManagement.EntityFrameworkCore;

public static class TenantManagementEfCoreQueryableExtensions
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
}
