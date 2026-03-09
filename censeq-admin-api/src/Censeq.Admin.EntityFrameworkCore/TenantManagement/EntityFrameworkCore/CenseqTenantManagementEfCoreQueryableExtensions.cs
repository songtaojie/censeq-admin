using Microsoft.EntityFrameworkCore;
using Censeq.Admin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Censeq.Admin.TenantManagement.EntityFrameworkCore;

public static class CenseqTenantManagementEfCoreQueryableExtensions
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
