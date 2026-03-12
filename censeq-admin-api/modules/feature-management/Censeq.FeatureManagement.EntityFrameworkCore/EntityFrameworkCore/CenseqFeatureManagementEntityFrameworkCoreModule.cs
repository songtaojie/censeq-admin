using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Censeq.FeatureManagement;

namespace Censeq.FeatureManagement.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqFeatureManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class CenseqFeatureManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<FeatureManagementDbContext>(options =>
        {
            options.AddRepository<FeatureGroupDefinitionRecord, EfCoreFeatureGroupDefinitionRecordRepository>();
            options.AddRepository<FeatureDefinitionRecord, EfCoreFeatureDefinitionRecordRepository>();
            options.AddDefaultRepositories<IFeatureManagementDbContext>();

            options.AddRepository<FeatureValue, EfCoreFeatureValueRepository>();
        });
    }
}
