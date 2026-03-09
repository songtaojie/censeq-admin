using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Censeq.Admin.Entities;
using Censeq.Admin.FeatureManagement;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Admin.EntityFrameworkCore.FeatureManagement;

public class EfCoreFeatureDefinitionRecordRepository :
    EfCoreRepository<CenseqAdminDbContext, FeatureDefinitionRecord, Guid>,
    IFeatureDefinitionRecordRepository
{
    public EfCoreFeatureDefinitionRecordRepository(
        IDbContextProvider<CenseqAdminDbContext> dbContextProvider)
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
