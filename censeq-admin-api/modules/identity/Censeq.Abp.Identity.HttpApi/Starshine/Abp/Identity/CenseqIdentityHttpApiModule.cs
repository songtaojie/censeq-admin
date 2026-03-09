using Volo.Abp.AspNetCore.Mvc;
using Censeq.Abp.Identity.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Localization.Resources.AbpUi;

namespace Censeq.Abp.Identity;

[DependsOn(typeof(CenseqIdentityApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class CenseqIdentityHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqIdentityHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<IdentityResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
