using Censeq.Admin.FeatureManagement;
using Censeq.Admin.FeatureManagement.JsonConverters;
using Censeq.Admin.FeatureManagement.Localization;
using Censeq.Admin.Localization;
using Censeq.AuditLogging;
using Censeq.SettingManagement;
using Censeq.TenantManagement;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Admin;

[DependsOn(
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpOpenIddictDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpJsonSystemTextJsonModule),
    typeof(AbpFeaturesModule),
    typeof(CenseqAuditLoggingDomainSharedModule),
    typeof(CenseqSettingManagementDomainSharedModule),
    typeof(CenseqTenantManagementDomainSharedModule)
    )]
public class CenseqAdminDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AdminGlobalFeatureConfigurator.Configure();
        AdminModuleExtensionConfigurator.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAdminDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<CenseqAdminResource>("zh-Hans")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Resources");
           
            options.DefaultResourceType = typeof(CenseqAdminResource);

            options.Resources
               .Add<CenseqFeatureManagementResource>("zh-Hans")
               .AddBaseTypes(typeof(AbpValidationResource))
               .AddVirtualJson("/FeatureManagement/Localization");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Censeq.Admin", typeof(CenseqAdminResource));
            options.MapCodeNamespace("Censeq.Admin.FeatureManagement", typeof(CenseqFeatureManagementResource));
        });

        var valueValidatorFactoryOptions = context.Services.GetPreConfigureActions<ValueValidatorFactoryOptions>();
        Configure<ValueValidatorFactoryOptions>(options =>
        {
            valueValidatorFactoryOptions.Configure(options);
        });

        Configure<AbpSystemTextJsonSerializerOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new StringValueTypeJsonConverter(valueValidatorFactoryOptions.Configure()));
        });

    }
}
