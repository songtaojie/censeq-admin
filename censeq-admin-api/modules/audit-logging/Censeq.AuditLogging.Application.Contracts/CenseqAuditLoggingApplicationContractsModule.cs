using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志应用程序契约模块。
/// </summary>
[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule),
    typeof(CenseqAuditLoggingDomainSharedModule)
)]
public class CenseqAuditLoggingApplicationContractsModule : AbpModule
{
}
