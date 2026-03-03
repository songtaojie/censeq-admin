using Starshine.Admin.FeatureManagement;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Starshine.Admin.EntityFrameworkCore.FeatureManagement;

public class EfCoreFeatureGroupDefinitionRecordRepository :
    EfCoreRepository<IFeatureManagementDbContext, FeatureGroupDefinitionRecord, Guid>,
    IFeatureGroupDefinitionRecordRepository
{
    public EfCoreFeatureGroupDefinitionRecordRepository(
        IDbContextProvider<IFeatureManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
