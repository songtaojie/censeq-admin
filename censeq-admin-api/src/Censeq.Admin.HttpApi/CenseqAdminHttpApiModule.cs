using Censeq.Admin.FeatureManagement;
using Censeq.Admin.FeatureManagement.JsonConverters;
using Censeq.Admin.FeatureManagement.Localization;
using Censeq.Admin.Localization;
using Censeq.SettingManagement;
using Censeq.TenantManagement;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.HttpApi;

namespace Censeq.Admin;

[DependsOn(
    typeof(AbpAccountHttpApiModule),
    typeof(CenseqAdminApplicationContractsModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(CenseqSettingManagementHttpApiModule),
    typeof(CenseqTenantManagementHttpApiModule)
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
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ControllersToRemove.Add(typeof(PermissionsController));
        });

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
                    typeof(CenseqFeatureManagementResource),
                    typeof(AbpUiResource)
                );
        });
    }
}
