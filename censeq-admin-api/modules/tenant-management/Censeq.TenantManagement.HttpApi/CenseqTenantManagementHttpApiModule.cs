using Censeq.FeatureManagement;
using Censeq.FeatureManagement.Localization;
using Censeq.TenantManagement.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.TenantManagement;

[DependsOn(
    typeof(CenseqTenantManagementApplicationContractsModule),
    typeof(CenseqFeatureManagementHttpApiModule),
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
                    typeof(CenseqFeatureManagementResource),
                    typeof(AbpUiResource));
        });
    }
}
