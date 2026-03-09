using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Censeq.Admin.FeatureManagement;
using Censeq.Admin.FeatureManagement.JsonConverters;
using Censeq.Admin.Localization;
using System.Collections.Generic;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;

namespace Censeq.Admin;

[DependsOn(
    typeof(AbpAccountHttpApiModule),
    typeof(CenseqAdminApplicationContractsModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
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
                    typeof(CenseqTenantManagementResource),
                    typeof(CenseqFeatureManagementResource),
                    typeof(AbpUiResource)
                );
        });
    }
}
