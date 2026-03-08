using Censeq.AuditLogging.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.AuditLogging.EntityFrameworkCore;

[ConnectionStringName(CenseqAuditLoggingDbProperties.ConnectionStringName)]
public interface IAuditLoggingDbContext : IEfCoreDbContext
{
    DbSet<AuditLog> AuditLogs { get; }
}
