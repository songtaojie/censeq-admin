using Censeq.AuditLogging.Entities;
using Censeq.AuditLogging.EntityFrameworkCore;
using Censeq.FeatureManagement.Entities;
using Censeq.FeatureManagement.EntityFrameworkCore;
using Censeq.Identity;
using Censeq.Identity.Entities;
using Censeq.Identity.EntityFrameworkCore;
using Censeq.Identity.EntityFrameworkCore.Modeling;
using Censeq.Admin.Menus;
using Censeq.Admin.Permissions;
using Censeq.LocalizationManagement.Entities;
using Censeq.LocalizationManagement.EntityFrameworkCore;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.EntityFrameworkCore;
using Censeq.OpenIddict.EntityFrameworkCore.Modeling;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;
using Censeq.PermissionManagement.Entities;
using Censeq.PermissionManagement.EntityFrameworkCore;
using Censeq.SettingManagement.Entities;
using Censeq.SettingManagement.EntityFrameworkCore;
using Censeq.SettingManagement.EntityFrameworkCore.Modeling;
using Censeq.TenantManagement.Entities;
using Censeq.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Admin.EntityFrameworkCore;

/// <summary>
/// 宿主统一 <see cref="DbContext"/>：实现各模块 <see cref="IEfCoreDbContext"/>，供单一迁移（如 InitialDb）覆盖全部表。
/// </summary>
[ConnectionStringName(ConnectionStrings.DefaultConnectionStringName)]
public class CenseqAdminDbContext(DbContextOptions<CenseqAdminDbContext> options)
    : AbpDbContext<CenseqAdminDbContext>(options),
        IAuditLoggingDbContext,
        ICenseqIdentityDbContext,
        ICenseqOpenIddictDbContext,
        IFeatureManagementDbContext,
        IPermissionManagementDbContext,
        ISettingManagementDbContext,
        ITenantManagementDbContext,
        ILocalizationManagementDbContext
{
    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    public DbSet<OpenIddictApplication> Applications { get; set; }
    public DbSet<OpenIddictAuthorization> Authorizations { get; set; }
    public DbSet<OpenIddictScope> Scopes { get; set; }
    public DbSet<OpenIddictToken> Tokens { get; set; }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuPermission> MenuPermissions { get; set; }

    public DbSet<TenantPermissionGrant> TenantPermissionGrants { get; set; }

    public DbSet<PermissionGroup> PermissionGroups { get; set; }
    public DbSet<PermissionDefinitionRecord> Permissions { get; set; }
    public DbSet<PermissionGrant> PermissionGrants { get; set; }

    public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; }
    public DbSet<FeatureDefinitionRecord> Features { get; set; }
    public DbSet<FeatureValue> FeatureValues { get; set; }

    public DbSet<Setting> Settings { get; set; }
    public DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; set; }

    public DbSet<LocalizationResource> LocalizationResources { get; set; }
    public DbSet<LocalizationCulture> LocalizationCultures { get; set; }
    public DbSet<LocalizationText> LocalizationTexts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureAdminMenus();
        builder.ConfigureOpenIddict();
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureTenantManagement();
        builder.ConfigureTenantPermissionGrants();
        builder.ConfigureLocalizationManagement();
    }
}
