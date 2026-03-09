using Censeq.Admin.Entities;
using Censeq.Admin.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Admin.FeatureManagement.EntityFrameworkCore;

public class EfCoreFeatureGroupDefinitionRecordRepository :
    EfCoreRepository<CenseqAdminDbContext, FeatureGroupDefinitionRecord, Guid>,
    IFeatureGroupDefinitionRecordRepository
{
    public EfCoreFeatureGroupDefinitionRecordRepository(
        IDbContextProvider<CenseqAdminDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
