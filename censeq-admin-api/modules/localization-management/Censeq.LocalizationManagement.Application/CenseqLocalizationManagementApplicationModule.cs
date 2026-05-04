using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Censeq.LocalizationManagement;

[DependsOn(
    typeof(CenseqLocalizationManagementDomainModule),
    typeof(CenseqLocalizationManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class CenseqLocalizationManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CenseqLocalizationManagementApplicationModule>();
        });
    }
}
