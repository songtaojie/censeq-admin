using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace Censeq.TenantManagement.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(CenseqTenantManagementDbProperties.ConnectionStringName)]
public interface ITenantManagementDbContext : IEfCoreDbContext
{
    DbSet<Tenant> Tenants { get; }

    DbSet<TenantConnectionString> TenantConnectionStrings { get; }
}
