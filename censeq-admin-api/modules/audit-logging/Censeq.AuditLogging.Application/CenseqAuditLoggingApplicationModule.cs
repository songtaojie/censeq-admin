using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志应用程序模块。
/// </summary>
[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(CenseqAuditLoggingApplicationContractsModule),
    typeof(CenseqAuditLoggingDomainModule)
)]
public class CenseqAuditLoggingApplicationModule : AbpModule
{
    /// <summary>
    /// 配置 CenseqAuditLoggingApplicationModule 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
