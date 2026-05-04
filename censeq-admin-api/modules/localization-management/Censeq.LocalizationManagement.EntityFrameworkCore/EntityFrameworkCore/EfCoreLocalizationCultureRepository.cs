using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.LocalizationManagement.EntityFrameworkCore;

public class EfCoreLocalizationCultureRepository
    : EfCoreRepository<ILocalizationManagementDbContext, LocalizationCulture, Guid>,
      ILocalizationCultureRepository
{
    public EfCoreLocalizationCultureRepository(
        IDbContextProvider<ILocalizationManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<LocalizationCulture?> FindAsync(
        string cultureName,
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(
                c => c.CultureName == cultureName && c.TenantId == tenantId,
                GetCancellationToken(cancellationToken));
    }

    public async Task<List<LocalizationCulture>> GetListAsync(
        Guid? tenantId,
        bool? isEnabled = null,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(c => c.TenantId == tenantId)
            .WhereIf(isEnabled.HasValue, c => c.IsEnabled == isEnabled!.Value)
            .OrderBy(c => c.CultureName)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
