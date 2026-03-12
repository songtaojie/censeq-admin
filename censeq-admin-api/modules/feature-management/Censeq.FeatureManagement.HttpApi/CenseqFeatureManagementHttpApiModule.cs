using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Censeq.FeatureManagement.JsonConverters;
using Censeq.FeatureManagement.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.FeatureManagement;

[DependsOn(
    typeof(CenseqFeatureManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CenseqFeatureManagementHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqFeatureManagementHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqFeatureManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });

        var valueValidatorFactoryOptions = context.Services.ExecutePreConfiguredActions<ValueValidatorFactoryOptions>();
        Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.AddIfNotContains(new StringValueTypeJsonConverter(valueValidatorFactoryOptions));
        });
    }
}
