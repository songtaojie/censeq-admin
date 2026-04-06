using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(CenseqAuditLoggingApplicationContractsModule)
)]
public class CenseqAuditLoggingHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
