using Censeq.PermissionManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Censeq.Framework.EntityFrameworkCore;
using Volo.Abp;

namespace Censeq.PermissionManagement.EntityFrameworkCore
{
    public static class PermissionManagementDbContextModelBuilderExtensions
    {
        public static void ConfigurePermissionManagement(this ModelBuilder builder)
        {
            Check.NotNull(builder, "builder");
            builder.Entity<PermissionGrant>(b => 
            {
                b.ToCenseqTable(nameof(PermissionGrant)).ConfigureCenseqByConvention();
                b.Property(x => x.Name).HasMaxLength(PermissionDefinitionRecordConsts.MaxNameLength).IsRequired();
                b.Property(x => x.ProviderName).HasMaxLength(PermissionGrantConsts.MaxProviderNameLength).IsRequired();
                b.Property(x => x.ProviderKey).HasMaxLength(PermissionGrantConsts.MaxProviderKeyLength).IsRequired();
                b.HasIndex(x => new { x.TenantId, x.Name, x.ProviderName, x.ProviderKey }).IsUnique();
                b.ApplyObjectExtensionMappings();
            });
            if (builder.IsHostDatabase())
            {
                builder.Entity<PermissionGroup>(b =>
                {
                    b.ToCenseqTable(nameof(PermissionGroup)).ConfigureCenseqByConvention();
                    b.Property(x => x.Name).HasMaxLength(PermissionGroupConsts.MaxNameLength).IsRequired();
                    b.Property(x => x.DisplayName).HasMaxLength(PermissionGroupConsts.MaxDisplayNameLength).IsRequired();
                    b.Property(x => x.LocalizationKey).HasMaxLength(PermissionGroupConsts.MaxLocalizationKeyLength);
                    b.HasIndex(x => new { x.Name }).IsUnique();
                    b.ApplyObjectExtensionMappings();
                });
                builder.Entity<PermissionDefinitionRecord>(b =>
                {
                    b.ToCenseqTable(nameof(PermissionDefinitionRecord)).ConfigureCenseqByConvention();
                    b.Property(x => x.GroupName).HasMaxLength(PermissionGroupConsts.MaxNameLength).IsRequired();
                    b.Property(x => x.Name).HasMaxLength(PermissionDefinitionRecordConsts.MaxNameLength).IsRequired();
                    b.Property(x => x.ParentName).HasMaxLength(PermissionDefinitionRecordConsts.MaxNameLength);
                    b.Property(x => x.DisplayName).HasMaxLength(PermissionDefinitionRecordConsts.MaxDisplayNameLength).IsRequired();
                    b.Property(x => x.LocalizationKey).HasMaxLength(PermissionDefinitionRecordConsts.MaxLocalizationKeyLength);
                    b.Property(x => x.Providers).HasMaxLength(PermissionDefinitionRecordConsts.MaxProvidersLength);
                    b.Property(x => x.StateCheckers).HasMaxLength(PermissionDefinitionRecordConsts.MaxStateCheckersLength);
                    b.HasIndex(x => new { x.Name }).IsUnique();
                    b.HasIndex(x => new { x.GroupName });
                    b.ApplyObjectExtensionMappings();
                });
            }
        }
    }
}
