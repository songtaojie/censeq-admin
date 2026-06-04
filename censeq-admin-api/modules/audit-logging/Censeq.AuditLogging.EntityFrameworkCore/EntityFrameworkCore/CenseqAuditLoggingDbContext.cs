using Censeq.AuditLogging.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.AuditLogging.EntityFrameworkCore;

/// <summary>
/// 审计日志数据库上下文。
/// </summary>
[ConnectionStringName(CenseqAuditLoggingDbProperties.ConnectionStringName)]
public class CenseqAuditLoggingDbContext(DbContextOptions<CenseqAuditLoggingDbContext> options) : AbpDbContext<CenseqAuditLoggingDbContext>(options), IAuditLoggingDbContext
{
    /// <summary>
    /// 审计日志集合。
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; }

    /// <summary>
    /// 创建模型时配置审计日志数据库模型。
    /// </summary>
    /// <param name="builder">builder。</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAuditLogging();
    }
}
