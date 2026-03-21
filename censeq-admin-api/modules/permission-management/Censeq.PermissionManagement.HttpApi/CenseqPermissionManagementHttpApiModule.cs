using Censeq.PermissionManagement.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.PermissionManagement;

[DependsOn(
    typeof(CenseqPermissionManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
    )]
public class CenseqPermissionManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqPermissionManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqPermissionManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
