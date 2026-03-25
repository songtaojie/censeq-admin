using Volo.Abp.Features;
using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Censeq.SettingManagement.Localization;


namespace Censeq.SettingManagement;

[DependsOn(typeof(AbpLocalizationModule),
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule))]
public class CenseqSettingManagementDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqSettingManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<CenseqSettingManagementResource>("zh-Hans")
                .AddBaseTypes(
                    typeof(AbpValidationResource)
                )
                .AddVirtualJson("/Censeq/SettingManagement/Localization/Resources");
        });
    }
}
