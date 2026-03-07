using Microsoft.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    internal static class StarshinePermissionManagementDbContextModelBuilderExtensions
    {
        public static void ConfigurePermissionManagement(this ModelBuilder builder)
        {
            Check.NotNull(builder, "builder");
            builder.Entity<PermissionGrant>(b => 
            {
                b.ToStarshineTable(nameof(PermissionGrant)).ConfigureStarshineByConvention();
                b.Property(x => x.Name).HasMaxLength(PermissionDefinitionRecordConsts.MaxNameLength).IsRequired();
                b.Property(x => x.ProviderName).HasMaxLength(PermissionGrantConsts.MaxProviderNameLength).IsRequired();
                b.Property(x => x.ProviderKey).HasMaxLength(PermissionGrantConsts.MaxProviderKeyLength).IsRequired();
                b.HasIndex(x => new { x.TenantId, x.Name, x.ProviderName, x.ProviderKey }).IsUnique();
                b.ApplyObjectExtensionMappings();
            });
            if (builder.IsHostDatabase())
            {
                builder.Entity<PermissionGroupDefinitionRecord>(b =>
                {
                    b.ToStarshineTable(nameof(PermissionGroupDefinitionRecord)).ConfigureStarshineByConvention();
                    b.Property(x => x.Name).HasMaxLength(PermissionGroupDefinitionRecordConsts.MaxNameLength).IsRequired();
                    b.Property(x => x.DisplayName).HasMaxLength(PermissionGroupDefinitionRecordConsts.MaxDisplayNameLength).IsRequired();
                    b.HasIndex(x => new { x.Name }).IsUnique();
                    b.ApplyObjectExtensionMappings();
                });
                builder.Entity<PermissionDefinitionRecord>(b =>
                {
                    b.ToStarshineTable(nameof(PermissionDefinitionRecord)).ConfigureStarshineByConvention();
                    b.Property(x => x.GroupName).HasMaxLength(PermissionGroupDefinitionRecordConsts.MaxNameLength).IsRequired();
                    b.Property(x => x.Name).HasMaxLength(PermissionDefinitionRecordConsts.MaxNameLength).IsRequired();
                    b.Property(x => x.ParentName).HasMaxLength(PermissionDefinitionRecordConsts.MaxNameLength);
                    b.Property(x => x.DisplayName).HasMaxLength(PermissionDefinitionRecordConsts.MaxDisplayNameLength).IsRequired();
                    b.Property(x => x.Providers).HasMaxLength(PermissionDefinitionRecordConsts.MaxProvidersLength);
                    b.Property(x => x.StateCheckers).HasMaxLength(PermissionDefinitionRecordConsts.MaxStateCheckersLength);
                    b.HasIndex(x => new { x.Name }).IsUnique();
                    b.HasIndex(x => new { x.GroupName });
                    b.ApplyObjectExtensionMappings();
                });
            }

            builder.TryConfigureObjectExtensions<PermissionManagementDbContext>();
        }
    }
}
