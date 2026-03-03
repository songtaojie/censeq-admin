using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Starshine.Admin.Entities;
using Starshine.Admin.FeatureManagement;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Starshine.Admin.EntityFrameworkCore.FeatureManagement;

public class EfCoreFeatureDefinitionRecordRepository :
    EfCoreRepository<StarshineAdminDbContext, FeatureDefinitionRecord, Guid>,
    IFeatureDefinitionRecordRepository
{
    public EfCoreFeatureDefinitionRecordRepository(
        IDbContextProvider<StarshineAdminDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<FeatureDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
