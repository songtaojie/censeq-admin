using Censeq.AuditLogging.Entities;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging.EntityFrameworkCore;

[DependsOn(typeof(CenseqAuditLoggingDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public class CenseqAuditLoggingEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqAuditLoggingDbContext>(options =>
        {
            options.AddRepository<AuditLog, EfCoreAuditLogRepository>();
        });
    }
}
