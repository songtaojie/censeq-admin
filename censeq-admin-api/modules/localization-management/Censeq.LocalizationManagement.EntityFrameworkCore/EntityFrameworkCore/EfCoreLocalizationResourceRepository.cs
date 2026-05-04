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

public class EfCoreLocalizationResourceRepository
    : EfCoreRepository<ILocalizationManagementDbContext, LocalizationResource, Guid>,
      ILocalizationResourceRepository
{
    public EfCoreLocalizationResourceRepository(
        IDbContextProvider<ILocalizationManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<LocalizationResource?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(r => r.Name == name, GetCancellationToken(cancellationToken));
    }

    public async Task<List<LocalizationResource>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(r => r.Name)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
