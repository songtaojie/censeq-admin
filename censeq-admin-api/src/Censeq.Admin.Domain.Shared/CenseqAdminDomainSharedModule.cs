using Censeq.Admin.FeatureManagement;
using Censeq.Admin.FeatureManagement.JsonConverters;
using Censeq.Admin.Localization;
using Censeq.AuditLogging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Identity;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Admin;

[DependsOn(
    typeof(AbpBackgroundJobsDomainSharedModule),
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpOpenIddictDomainSharedModule),
    typeof(AbpPermissionManagementDomainSharedModule),
    typeof(AbpSettingManagementDomainSharedModule),
    typeof(AbpJsonSystemTextJsonModule),
    typeof(CenseqAuditLoggingDomainSharedModule)
    )]
public class StarshineAdminDomainSharedModule : AbpModule
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
            options.FileSets.AddEmbedded<StarshineAdminDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<StarshineAdminResource>("zh-Hans")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Resources");
            options.Resources
                .Add<StarshineTenantManagementResource>("zh-Hans")
                .AddBaseTypes(
                    typeof(AbpValidationResource)
                ).AddVirtualJson("/Localization/TenantResources");
            options.DefaultResourceType = typeof(StarshineAdminResource);

            options.Resources
               .Add<StarshineFeatureManagementResource>("zh-Hans")
               .AddBaseTypes(typeof(AbpValidationResource))
               .AddVirtualJson("/Localization/FeatureManagementResources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Censeq.Admin", typeof(StarshineAdminResource));
            options.MapCodeNamespace("Censeq.Admin.TenantManagement", typeof(StarshineTenantManagementResource));
            options.MapCodeNamespace("Censeq.Admin.FeatureManagement", typeof(StarshineFeatureManagementResource));
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
