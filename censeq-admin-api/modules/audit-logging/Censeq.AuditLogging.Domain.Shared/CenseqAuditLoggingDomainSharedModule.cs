using Censeq.AuditLogging.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志领域共享模块。
/// </summary>
[DependsOn(typeof(AbpValidationModule))]
public class CenseqAuditLoggingDomainSharedModule : AbpModule
{
    /// <summary>
    /// 配置 CenseqAuditLoggingDomainSharedModule 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
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
