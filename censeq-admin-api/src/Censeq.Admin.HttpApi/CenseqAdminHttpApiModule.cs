using Censeq.Account;
using Censeq.Admin.Localization;
using Censeq.FeatureManagement;
using Censeq.FeatureManagement.JsonConverters;
using Censeq.FeatureManagement.Localization;
using Censeq.Identity;
using Censeq.PermissionManagement;
using Censeq.SettingManagement;
using Censeq.TenantManagement;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.Admin;

[DependsOn(
    typeof(CenseqAccountHttpApiModule),
    typeof(CenseqAdminApplicationContractsModule),
    typeof(CenseqIdentityHttpApiModule),
    typeof(CenseqPermissionManagementHttpApiModule),
    typeof(CenseqSettingManagementHttpApiModule),
    typeof(CenseqTenantManagementHttpApiModule),
    typeof(CenseqFeatureManagementHttpApiModule)
    )]
public class CenseqAdminHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqAdminHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
        var valueValidatorFactoryOptions = context.Services.ExecutePreConfiguredActions<ValueValidatorFactoryOptions>();
        Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.AddIfNotContains(new StringValueTypeJsonConverter(valueValidatorFactoryOptions));
        });

    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqAdminResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
