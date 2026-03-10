using Censeq.AuditLogging.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.AuditLogging;

[DependsOn(typeof(AbpValidationModule))]
public class CenseqAuditLoggingDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAuditLoggingDomainSharedModule>();
        });
        
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources.Add<CenseqAuditLoggingResource>("zh-Hans")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/Resources");
        });
    }
}
