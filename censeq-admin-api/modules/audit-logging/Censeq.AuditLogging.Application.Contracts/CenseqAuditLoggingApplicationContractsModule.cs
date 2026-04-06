using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule),
    typeof(CenseqAuditLoggingDomainSharedModule)
)]
public class CenseqAuditLoggingApplicationContractsModule : AbpModule
{
}
