using Censeq.AuditLogging.Entities;
using Censeq.AuditLogging.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.AuditLogging.EntityFrameworkCore;

[ConnectionStringName(CenseqAuditLoggingDbProperties.ConnectionStringName)]
public class CenseqAuditLoggingDbContext(DbContextOptions<CenseqAuditLoggingDbContext> options) : AbpDbContext<CenseqAuditLoggingDbContext>(options), IAuditLoggingDbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAuditLogging();
    }
}
