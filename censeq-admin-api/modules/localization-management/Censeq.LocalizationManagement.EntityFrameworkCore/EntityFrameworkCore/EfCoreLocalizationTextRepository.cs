using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.LocalizationManagement.EntityFrameworkCore;

public class EfCoreLocalizationTextRepository
    : EfCoreRepository<ILocalizationManagementDbContext, LocalizationText, Guid>,
      ILocalizationTextRepository
{
    public EfCoreLocalizationTextRepository(
        IDbContextProvider<ILocalizationManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<LocalizationText?> FindAsync(
        string resourceName,
        string cultureName,
        string key,
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(
                t => t.ResourceName == resourceName
                     && t.CultureName == cultureName
                     && t.Key == key
                     && t.TenantId == tenantId,
                GetCancellationToken(cancellationToken));
    }

    public async Task<List<LocalizationText>> GetListAsync(
        string resourceName,
        string cultureName,
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(t => t.ResourceName == resourceName
                        && t.CultureName == cultureName
                        && t.TenantId == tenantId)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? resourceName = null,
        string? cultureName = null,
        Guid? tenantId = null,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        return await (await BuildQueryAsync(resourceName, cultureName, tenantId, filter))
            .LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<LocalizationText>> GetPagedListAsync(
        string? resourceName = null,
        string? cultureName = null,
        Guid? tenantId = null,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = 20,
        string? sorting = null,
        CancellationToken cancellationToken = default)
    {
        var query = await BuildQueryAsync(resourceName, cultureName, tenantId, filter);

        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? $"{nameof(LocalizationText.ResourceName)},{nameof(LocalizationText.Key)}" : sorting!)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<string>> GetDistinctCultureNamesAsync(
        string resourceName,
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(t => t.ResourceName == resourceName && t.TenantId == tenantId)
            .Select(t => t.CultureName)
            .Distinct()
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    private async Task<IQueryable<LocalizationText>> BuildQueryAsync(
        string? resourceName,
        string? cultureName,
        Guid? tenantId,
        string? filter)
    {
        return (await GetDbSetAsync())
            .WhereIf(!resourceName.IsNullOrWhiteSpace(), t => t.ResourceName == resourceName)
            .WhereIf(!cultureName.IsNullOrWhiteSpace(), t => t.CultureName == cultureName)
            .Where(t => t.TenantId == tenantId)
            .WhereIf(!filter.IsNullOrWhiteSpace(),
                t => t.Key.Contains(filter!) || (t.Value != null && t.Value.Contains(filter!)));
    }
}
