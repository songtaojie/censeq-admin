using Censeq.AuditLogging.Entities;
using Censeq.Framework.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging.EntityFrameworkCore;

[DependsOn(typeof(CenseqAuditLoggingDomainModule))]
[DependsOn(typeof(CenseqEntityFrameworkCoreModule))]
public class CenseqAuditLoggingEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqAuditLoggingDbContext>(options =>
        {
            options.AddRepository<AuditLog, EfCoreAuditLogRepository>();
        });

        var configuration = context.Services.GetConfiguration();
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<CenseqAuditLoggingDbContext>(dbContext =>
            {
                dbContext.UseDynamicSql(configuration);
            });
        });

    }
}
