using Censeq.Admin.FeatureManagement;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Admin.EntityFrameworkCore.FeatureManagement;

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
