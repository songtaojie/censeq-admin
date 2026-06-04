using Censeq.AuditLogging.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.AuditLogging.EntityFrameworkCore;

/// <summary>
/// 审计日志数据库上下文接口。
/// </summary>
[ConnectionStringName(CenseqAuditLoggingDbProperties.ConnectionStringName)]
public interface IAuditLoggingDbContext : IEfCoreDbContext
{
    /// <summary>
    /// 审计日志集合。
    /// </summary>
    DbSet<AuditLog> AuditLogs { get; }
}
