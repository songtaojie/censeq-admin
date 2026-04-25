using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Identity.EntityFrameworkCore;

/// <summary>
/// Ef核心身份声明类型仓储
/// </summary>
public class EfCoreIdentityClaimTypeRepository : EfCoreRepository<ICenseqIdentityDbContext, IdentityClaimType, Guid>, IIdentityClaimTypeRepository
{
    public EfCoreIdentityClaimTypeRepository(IDbContextProvider<ICenseqIdentityDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// Task<bool>
    /// </summary>
    public virtual async Task<bool> AnyAsync(
        string name,
        Guid? ignoredId = null,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(ignoredId != null, ct => ct.Id != ignoredId)
            .CountAsync(ct => ct.Name == name, GetCancellationToken(cancellationToken)) > 0;
    }

    /// <summary>
    /// Task<List<Identity声明Type>>
    /// </summary>
    public virtual async Task<List<IdentityClaimType>> GetListAsync(
        string? sorting,
        int maxResultCount,
        int skipCount,
        string filter,
        CancellationToken cancellationToken = default)
    {
        var identityClaimTypes = await (await GetDbSetAsync())
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(filter)
                    || (u.Description != null && u.Description.Contains(filter))
                    || (u.Regex != null && u.Regex.Contains(filter))
                    || (u.RegexDescription != null && u.RegexDescription.Contains(filter))
            )
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(IdentityClaimType.Name) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));

        return identityClaimTypes;
    }

    /// <summary>
    /// Task<long>
    /// </summary>
    public virtual async Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(filter!)
                    || (u.Description != null && u.Description.Contains(filter!))
                    || (u.Regex != null && u.Regex.Contains(filter!))
                    || (u.RegexDescription != null && u.RegexDescription.Contains(filter!))
            ).LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// Task<List<Identity声明Type>>
    /// </summary>
    public virtual async Task<List<IdentityClaimType>> GetListByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => names.Contains(x.Name))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
