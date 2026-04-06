using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(CenseqAuditLoggingApplicationContractsModule),
    typeof(CenseqAuditLoggingDomainModule)
)]
public class CenseqAuditLoggingApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
