using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志 HTTP API 模块。
/// </summary>
[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(CenseqAuditLoggingApplicationContractsModule)
)]
public class CenseqAuditLoggingHttpApiModule : AbpModule
{
    /// <summary>
    /// 配置 CenseqAuditLoggingHttpApiModule 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
