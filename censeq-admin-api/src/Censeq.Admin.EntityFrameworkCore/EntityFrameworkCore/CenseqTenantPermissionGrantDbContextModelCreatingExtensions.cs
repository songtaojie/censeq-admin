using Censeq.Admin.Permissions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Censeq.Admin.EntityFrameworkCore;

public static class TenantPermissionGrantDbContextModelCreatingExtensions
{
    public static void ConfigureTenantPermissionGrants(this ModelBuilder builder)
    {
        builder.Entity<TenantPermissionGrant>(b =>
        {
            b.ToTable("censeq_tenant_permission_grants");
            b.ConfigureByConvention();
            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.PermissionName).IsRequired().HasMaxLength(128);
            b.HasIndex(x => new { x.TenantId, x.PermissionName }).IsUnique();
        });
    }
}
