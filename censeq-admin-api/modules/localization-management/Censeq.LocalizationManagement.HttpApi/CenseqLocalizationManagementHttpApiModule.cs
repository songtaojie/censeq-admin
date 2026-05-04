using Censeq.LocalizationManagement.Localization;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.LocalizationManagement;

[DependsOn(
    typeof(CenseqLocalizationManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class CenseqLocalizationManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqLocalizationManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqLocalizationManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
