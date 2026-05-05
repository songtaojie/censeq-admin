using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Censeq.SettingManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.SettingManagement.EntityFrameworkCore;

public class EfCoreSettingDefinitionRecordRepository : EfCoreRepository<ISettingManagementDbContext, SettingDefinitionRecord, Guid>, ISettingDefinitionRecordRepository
{
    public EfCoreSettingDefinitionRecordRepository(IDbContextProvider<ISettingManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<SettingDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public virtual async Task<List<SettingDefinitionRecord>> GetPagedListAsync(string? filter, int skipCount, int maxResultCount, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(filter),
                x => x.Name.Contains(filter!) || x.DisplayName.Contains(filter!))
            .OrderBy(x => x.Name)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filter, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(filter),
                x => x.Name.Contains(filter!) || x.DisplayName.Contains(filter!))
            .LongCountAsync(cancellationToken);
    }
}
