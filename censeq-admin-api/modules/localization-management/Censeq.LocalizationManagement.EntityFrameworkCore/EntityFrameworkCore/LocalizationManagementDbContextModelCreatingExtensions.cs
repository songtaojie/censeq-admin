using Censeq.LocalizationManagement.Entities;
using Censeq.Framework.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Censeq.LocalizationManagement.EntityFrameworkCore;

public static class LocalizationManagementDbContextModelCreatingExtensions
{
    public static void ConfigureLocalizationManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<LocalizationResource>(b =>
        {
            b.ToCenseqTable(nameof(LocalizationResource)).ConfigureCenseqByConvention();

            b.Property(x => x.Name)
                .HasMaxLength(LocalizationResourceConsts.MaxNameLength)
                .IsRequired();

            b.Property(x => x.DisplayName)
                .HasMaxLength(LocalizationResourceConsts.MaxDisplayNameLength);

            b.Property(x => x.DefaultCultureName)
                .HasMaxLength(LocalizationResourceConsts.MaxDefaultCultureNameLength);

            b.HasIndex(x => x.Name).IsUnique();

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<LocalizationCulture>(b =>
        {
            b.ToCenseqTable(nameof(LocalizationCulture)).ConfigureCenseqByConvention();

            b.Property(x => x.CultureName)
                .HasMaxLength(LocalizationCultureConsts.MaxCultureNameLength)
                .IsRequired();

            b.Property(x => x.UiCultureName)
                .HasMaxLength(LocalizationCultureConsts.MaxUiCultureNameLength);

            b.Property(x => x.DisplayName)
                .HasMaxLength(LocalizationCultureConsts.MaxDisplayNameLength)
                .IsRequired();

            // 同一租户下语言代码唯一
            b.HasIndex(x => new { x.TenantId, x.CultureName }).IsUnique();

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<LocalizationText>(b =>
        {
            b.ToCenseqTable(nameof(LocalizationText)).ConfigureCenseqByConvention();

            b.Property(x => x.ResourceName)
                .HasMaxLength(LocalizationTextConsts.MaxResourceNameLength)
                .IsRequired();

            b.Property(x => x.CultureName)
                .HasMaxLength(LocalizationTextConsts.MaxCultureNameLength)
                .IsRequired();

            b.Property(x => x.Key)
                .HasMaxLength(LocalizationTextConsts.MaxKeyLength)
                .IsRequired();

            // 同一租户+资源+语言+键 唯一
            b.HasIndex(x => new { x.TenantId, x.ResourceName, x.CultureName, x.Key }).IsUnique();

            b.ApplyObjectExtensionMappings();
        });
    }
}
