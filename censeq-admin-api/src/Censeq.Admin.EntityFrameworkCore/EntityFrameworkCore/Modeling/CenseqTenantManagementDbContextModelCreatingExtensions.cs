using Microsoft.EntityFrameworkCore;
using Censeq.Admin.Consts;
using Censeq.Admin.Entities;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using static Censeq.Admin.Consts.TenantConsts;

namespace Censeq.Admin.EntityFrameworkCore.Modeling
{
    internal static class StarshineTenantManagementDbContextModelCreatingExtensions
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
                b.ToStarshineTable(nameof(Tenant))
                    .ConfigureStarshineByConvention();
                b.Property(t => t.Name).IsRequired().HasMaxLength(TenantConsts.MaxNameLength);
                b.Property(t => t.NormalizedName).IsRequired().HasMaxLength(TenantConsts.MaxNameLength);
                //b.HasMany(u => u.ConnectionStrings).WithOne().HasForeignKey(uc => uc.TenantId).IsRequired();
                b.HasIndex(u => u.Name);
                b.HasIndex(u => u.NormalizedName);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<TenantConnectionString>(b =>
            {
                b.ToStarshineTable(nameof(TenantConnectionString));
                b.Property(t => t.TenantId).HasComment("租户ID");
                b.HasKey(x => new { x.TenantId, x.Name });
                b.Property(cs => cs.Name).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxNameLength);
                b.Property(cs => cs.Value).IsRequired().HasMaxLength(TenantConnectionStringConsts.MaxValueLength);
                b.ApplyObjectExtensionMappings();
            });
        }
    }
}
