using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Censeq.TenantManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.TenantManagement.EntityFrameworkCore;

public class EfCoreTenantRepository : EfCoreRepository<ITenantManagementDbContext, Tenant, Guid>, ITenantRepository
{
    public EfCoreTenantRepository(IDbContextProvider<ITenantManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    public virtual async Task<Tenant?> FindByNameAsync(
        string normalizedName,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .IncludeDetails(includeDetails)
            .OrderBy(t => t.Id)
            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, GetCancellationToken(cancellationToken));
    }

    public virtual async Task<Tenant?> FindByCodeAsync(
        string code,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        var normalized = Tenant.NormalizeCode(code);
        if (normalized == null)
        {
            return null;
        }

        return await (await GetDbSetAsync())
            .IncludeDetails(includeDetails)
            .OrderBy(t => t.Id)
            .FirstOrDefaultAsync(t => t.Code == normalized, GetCancellationToken(cancellationToken));
    }

    [Obsolete("Use FindByNameAsync method.")]
    public virtual Tenant? FindByName(string normalizedName, bool includeDetails = true)
    {
        return DbSet
            .IncludeDetails(includeDetails)
            .OrderBy(t => t.Id)
            .FirstOrDefault(t => t.NormalizedName == normalizedName);
    }

    [Obsolete("Use FindAsync method.")]
    public virtual Tenant? FindById(Guid id, bool includeDetails = true)
    {
        return DbSet
            .IncludeDetails(includeDetails)
            .OrderBy(t => t.Id)
            .FirstOrDefault(t => t.Id == id);
    }

    public virtual async Task<List<Tenant>> GetListAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .IncludeDetails(includeDetails)
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(filter!) ||
                    (u.Code != null && u.Code.Contains(filter!))
            )
            .OrderBy(sorting.IsNullOrEmpty() ? nameof(Tenant.Name) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(filter!) ||
                    (u.Code != null && u.Code.Contains(filter!))
            ).CountAsync(cancellationToken: GetCancellationToken(cancellationToken));
    }

    [Obsolete("Use WithDetailsAsync method.")]
    public override IQueryable<Tenant> WithDetails()
    {
        return GetQueryable().IncludeDetails();
    }

    public override async Task<IQueryable<Tenant>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
