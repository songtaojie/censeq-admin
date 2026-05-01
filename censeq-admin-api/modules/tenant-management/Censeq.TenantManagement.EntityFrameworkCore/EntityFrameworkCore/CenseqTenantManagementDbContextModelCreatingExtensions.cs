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
                b.Property(t => t.Code).IsRequired(false).HasMaxLength(MaxCodeLength);
                b.Property(t => t.Domain).IsRequired(false).HasMaxLength(MaxDomainLength);
                b.Property(t => t.Icon).IsRequired(false).HasMaxLength(MaxIconLength);
                b.Property(t => t.Copyright).IsRequired(false).HasMaxLength(MaxCopyrightLength);
                b.Property(t => t.IcpNo).IsRequired(false).HasMaxLength(MaxIcpNoLength);
                b.Property(t => t.IcpAddress).IsRequired(false).HasMaxLength(MaxIcpAddressLength);
                b.Property(t => t.Remark).IsRequired(false).HasMaxLength(MaxRemarkLength);
                b.Property(t => t.MaxUserCount).IsRequired().HasDefaultValue(0);
                b.Property(t => t.IsActive).IsRequired().HasDefaultValue(true);
                //b.HasMany(u => u.ConnectionStrings).WithOne().HasForeignKey(uc => uc.TenantId).IsRequired();
                b.HasIndex(u => u.Name);
                b.HasIndex(u => u.NormalizedName);
                b.HasIndex(u => u.Code).IsUnique();
                b.HasIndex(u => u.Domain).IsUnique();
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
