using Censeq.AuditLogging.Entities;
using Censeq.Framework.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.AuditLogging.EntityFrameworkCore;

/// <summary>
/// 审计日志 Entity Framework Core 模块。
/// </summary>
[DependsOn(typeof(CenseqAuditLoggingDomainModule))]
[DependsOn(typeof(CenseqEntityFrameworkCoreModule))]
public class CenseqAuditLoggingEntityFrameworkCoreModule : AbpModule
{
    /// <summary>
    /// 配置 CenseqAuditLoggingEntityFrameworkCoreModule 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqAuditLoggingDbContext>(options =>
        {
            options.AddRepository<AuditLog, EfCoreAuditLogRepository>();
        });
    }
}
