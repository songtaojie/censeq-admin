using Localization.Resources.AbpUi;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Censeq.TenantManagement.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Censeq.TenantManagement;

[DependsOn(
    typeof(CenseqTenantManagementApplicationContractsModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpAspNetCoreMvcModule)
    )]
public class CenseqTenantManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqTenantManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqTenantManagementResource>()
                .AddBaseTypes(
                    typeof(AbpFeatureManagementResource),
                    typeof(AbpUiResource));
        });
    }
}
