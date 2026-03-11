using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Censeq.TenantManagement.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.TenantManagement;

[DependsOn(typeof(AbpValidationModule))]
public class CenseqTenantManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqTenantManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<CenseqTenantManagementResource>("en")
                .AddBaseTypes(
                    typeof(AbpValidationResource)
                ).AddVirtualJson("/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Censeq.TenantManagement", typeof(CenseqTenantManagementResource));
        });
    }
}
