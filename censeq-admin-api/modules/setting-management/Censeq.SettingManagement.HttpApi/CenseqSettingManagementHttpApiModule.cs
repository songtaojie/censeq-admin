using Censeq.SettingManagement.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.SettingManagement;

[DependsOn(
    typeof(CenseqSettingManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CenseqSettingManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqSettingManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqSettingManagementResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
