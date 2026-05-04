using Censeq.LocalizationManagement.Entities;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.LocalizationManagement.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqLocalizationManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class CenseqLocalizationManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<LocalizationManagementDbContext>(options =>
        {
            options.AddRepository<LocalizationResource, EfCoreLocalizationResourceRepository>();
            options.AddRepository<LocalizationCulture, EfCoreLocalizationCultureRepository>();
            options.AddRepository<LocalizationText, EfCoreLocalizationTextRepository>();
            options.AddDefaultRepositories<ILocalizationManagementDbContext>();
        });
    }
}
