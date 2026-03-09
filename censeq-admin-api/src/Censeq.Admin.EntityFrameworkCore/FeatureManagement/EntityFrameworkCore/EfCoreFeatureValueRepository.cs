using Censeq.Admin.Entities;
using Censeq.Admin.EntityFrameworkCore;
using Censeq.Admin.FeatureManagement;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Admin.FeatureManagement.EntityFrameworkCore;

public class EfCoreFeatureValueRepository : EfCoreRepository<CenseqAdminDbContext, FeatureValue, Guid>, IFeatureValueRepository
{
    public EfCoreFeatureValueRepository(IDbContextProvider<CenseqAdminDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<FeatureValue?> FindAsync(
        string name,
        string providerName,
        string? providerKey,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey, GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<FeatureValue>> FindAllAsync(
        string name,
        string providerName,
        string? providerKey,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(
                s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey
            ).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<FeatureValue>> GetListAsync(
        string? providerName,
        string? providerKey,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(
                s => s.ProviderName == providerName && s.ProviderKey == providerKey
            ).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task DeleteAsync(
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        await DeleteAsync(s => s.ProviderName == providerName && s.ProviderKey == providerKey, cancellationToken: GetCancellationToken(cancellationToken));
    }
}
