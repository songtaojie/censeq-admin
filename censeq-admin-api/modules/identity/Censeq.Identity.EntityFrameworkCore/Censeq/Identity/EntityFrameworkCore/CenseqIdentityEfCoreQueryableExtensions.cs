using System.Linq;
using Censeq.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace Censeq.Identity.EntityFrameworkCore;

/// <summary>
/// Censeq身份Ef核心可查询扩展
/// </summary>
public static class CenseqIdentityEfCoreQueryableExtensions
{
    /// <summary>
    /// IQueryable<IdentityUser>
    /// </summary>
    public static IQueryable<IdentityUser> IncludeDetails(this IQueryable<IdentityUser> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.Roles)
            .Include(x => x.Logins)
            .Include(x => x.Claims)
            .Include(x => x.Tokens)
            .Include(x => x.OrganizationUnits);
    }

    /// <summary>
    /// IQueryable<IdentityRole>
    /// </summary>
    public static IQueryable<IdentityRole> IncludeDetails(this IQueryable<IdentityRole> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.Claims);
    }

    /// <summary>
    /// IQueryable<OrganizationUnit>
    /// </summary>
    public static IQueryable<OrganizationUnit> IncludeDetails(this IQueryable<OrganizationUnit> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.Roles);
    }
}
