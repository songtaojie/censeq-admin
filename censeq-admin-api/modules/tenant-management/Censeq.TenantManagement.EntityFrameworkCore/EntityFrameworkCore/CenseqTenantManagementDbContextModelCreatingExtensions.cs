using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Censeq.Framework.EntityFrameworkCore;
using static Censeq.TenantManagement.TenantConsts;
using Censeq.TenantManagement.Entities;

namespace Censeq.TenantManagement.EntityFrameworkCore
{
    public static class CenseqTenantManagementDbContextModelCreatingExtensions
    {
        public static void ConfigureTenantManagement(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (builder.IsTenantOnlyDatabase())
            {
                return;
            }

            builder.Entity<Tenant>(b =>
            {
                b.ToCenseqTable(nameof(Tenant))
                    .ConfigureCenseqByConvention();
                b.Property(t => t.Name).IsRequired().HasMaxLength(MaxNameLength);
                b.Property(t => t.NormalizedName).IsRequired().HasMaxLength(MaxNameLength);
                //b.HasMany(u => u.ConnectionStrings).WithOne().HasForeignKey(uc => uc.TenantId).IsRequired();
                b.HasIndex(u => u.Name);
                b.HasIndex(u => u.NormalizedName);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<TenantConnectionString>(b =>
            {
                b.ToCenseqTable(nameof(TenantConnectionString));
                b.Property(t => t.TenantId).HasComment("租户ID");
                b.HasKey(x => new { x.TenantId, x.Name });
                b.Property(cs => cs.Name).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxNameLength);
                b.Property(cs => cs.Value).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxValueLength);
                b.ApplyObjectExtensionMappings();
            });
        }
    }
}
