using Censeq.Framework.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Censeq.Admin.EntityFrameworkCore;

public static class CenseqAdminMenuDbContextModelCreatingExtensions
{
    public static void ConfigureAdminMenus(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Menus.Menu>(b =>
        {
            b.ToCenseqTable(nameof(Menus.Menu)).ConfigureCenseqByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(Menus.MenuConsts.MaxNameLength);
            b.Property(x => x.Title).IsRequired().HasMaxLength(Menus.MenuConsts.MaxTitleLength);
            b.Property(x => x.RouteName).HasMaxLength(Menus.MenuConsts.MaxRouteNameLength);
            b.Property(x => x.Path).HasMaxLength(Menus.MenuConsts.MaxPathLength);
            b.Property(x => x.Component).HasMaxLength(Menus.MenuConsts.MaxComponentLength);
            b.Property(x => x.Redirect).HasMaxLength(Menus.MenuConsts.MaxPathLength);
            b.Property(x => x.Icon).HasMaxLength(Menus.MenuConsts.MaxIconLength);
            b.Property(x => x.ExternalUrl).HasMaxLength(Menus.MenuConsts.MaxExternalUrlLength);
            b.Property(x => x.Remark).HasMaxLength(Menus.MenuConsts.MaxRemarkLength);
            b.Property(x => x.ButtonCode).HasMaxLength(Menus.MenuConsts.MaxButtonCodeLength);
            b.Property(x => x.PermissionGroups).HasMaxLength(Menus.MenuConsts.MaxPermissionGroupsLength);
            b.Property(x => x.Type).HasConversion<byte>().IsRequired();
            b.Property(x => x.AuthorizationMode).HasConversion<byte>().IsRequired();
            b.HasIndex(x => new { x.TenantId, x.ParentId, x.Sort });
            b.HasIndex(x => new { x.TenantId, x.Path }).IsUnique();
            b.HasIndex(x => new { x.TenantId, x.RouteName }).IsUnique();
            b.HasIndex(x => new { x.TenantId, x.ParentId, x.Name }).IsUnique();
            b.HasOne<Menus.Menu>().WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Permissions).WithOne().HasForeignKey(x => x.MenuId).OnDelete(DeleteBehavior.Cascade);
            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<Menus.MenuPermission>(b =>
        {
            b.ToCenseqTable(nameof(Menus.MenuPermission)).ConfigureCenseqByConvention();
            b.Property(x => x.PermissionName).IsRequired().HasMaxLength(Menus.MenuConsts.MaxNameLength);
            b.HasIndex(x => new { x.MenuId, x.PermissionName }).IsUnique();
            b.ApplyObjectExtensionMappings();
        });
    }
}