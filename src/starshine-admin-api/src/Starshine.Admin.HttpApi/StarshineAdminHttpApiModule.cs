using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Starshine.Admin.Localization;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.Localization;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

namespace Starshine.Admin;

[DependsOn(
    typeof(AbpAccountHttpApiModule),
    typeof(StarshineAdminApplicationContractsModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
    )]
public class StarshineAdminHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(StarshineAdminHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ControllersToRemove.Add(typeof(PermissionsController));
        });

    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<StarshineAdminResource>()
                .AddBaseTypes(
                    typeof(StarshineTenantManagementResource),
                    typeof(AbpFeatureManagementResource),
                    typeof(AbpUiResource)
                );
        });
    }
}
