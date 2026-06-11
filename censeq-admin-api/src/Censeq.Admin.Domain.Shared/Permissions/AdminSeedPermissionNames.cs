using System;
using System.Collections.Generic;
using System.Linq;

namespace Censeq.Admin.Permissions;

public static class AdminSeedPermissionNames
{
    public static IReadOnlyList<string> UserMenu { get; } = ["CenseqIdentity.Users"];
    public static IReadOnlyList<string> UserCreate { get; } = ["CenseqIdentity.Users.Create"];
    public static IReadOnlyList<string> UserUpdate { get; } = ["CenseqIdentity.Users.Update"];
    public static IReadOnlyList<string> UserDelete { get; } = ["CenseqIdentity.Users.Delete"];
    public static IReadOnlyList<string> UserManageRoles { get; } = ["CenseqIdentity.Users.Update.ManageRoles"];
    public static IReadOnlyList<string> UserManagePermissions { get; } = ["CenseqIdentity.Users.ManagePermissions"];

    public static IReadOnlyList<string> OrganizationUnitMenu { get; } = ["CenseqIdentity.OrganizationUnits"];
    public static IReadOnlyList<string> OrganizationUnitCreate { get; } = ["CenseqIdentity.OrganizationUnits.Create"];
    public static IReadOnlyList<string> OrganizationUnitUpdate { get; } = ["CenseqIdentity.OrganizationUnits.Update"];
    public static IReadOnlyList<string> OrganizationUnitDelete { get; } = ["CenseqIdentity.OrganizationUnits.Delete"];

    public static IReadOnlyList<string> RoleMenu { get; } = ["CenseqIdentity.Roles"];
    public static IReadOnlyList<string> RoleCreate { get; } = ["CenseqIdentity.Roles.Create"];
    public static IReadOnlyList<string> RoleUpdate { get; } = ["CenseqIdentity.Roles.Update"];
    public static IReadOnlyList<string> RoleDelete { get; } = ["CenseqIdentity.Roles.Delete"];
    public static IReadOnlyList<string> RoleManagePermissions { get; } = ["CenseqIdentity.Roles.ManagePermissions"];

    public static IReadOnlyList<string> ClaimTypeMenu { get; } = ["CenseqIdentity.ClaimTypes"];
    public static IReadOnlyList<string> ClaimTypeCreate { get; } = ["CenseqIdentity.ClaimTypes.Create"];
    public static IReadOnlyList<string> ClaimTypeUpdate { get; } = ["CenseqIdentity.ClaimTypes.Update"];
    public static IReadOnlyList<string> ClaimTypeDelete { get; } = ["CenseqIdentity.ClaimTypes.Delete"];

    public static IReadOnlyList<string> SecurityLogMenu { get; } = ["CenseqIdentity.SecurityLogs"];
    public static IReadOnlyList<string> SecurityLogDelete { get; } = ["CenseqIdentity.SecurityLogs.Delete"];

    public static IReadOnlyList<string> SessionMenu { get; } = ["CenseqIdentity.Sessions"];
    public static IReadOnlyList<string> SessionManage { get; } = ["CenseqIdentity.Sessions.Manage"];
    public static IReadOnlyList<string> SessionRevoke { get; } = ["CenseqIdentity.Sessions.Revoke"];

    public static IReadOnlyList<string> MenuManagementMenu { get; } = ["CenseqAdmin.Menus"];
    public static IReadOnlyList<string> MenuManagementCreate { get; } = ["CenseqAdmin.Menus.Create"];
    public static IReadOnlyList<string> MenuManagementUpdate { get; } = ["CenseqAdmin.Menus.Update"];
    public static IReadOnlyList<string> MenuManagementDelete { get; } = ["CenseqAdmin.Menus.Delete"];
    public static IReadOnlyList<string> MenuManagementManageStatus { get; } = ["CenseqAdmin.Menus.ManageStatus"];
    public static IReadOnlyList<string> MenuManagementManageOrder { get; } = ["CenseqAdmin.Menus.ManageOrder"];
    public static IReadOnlyList<string> MenuCopyFromHost { get; } = ["CenseqAdmin.Menus.CopyFromHost"];

    public static IReadOnlyList<string> FileManagementMenu { get; } = ["CenseqAdmin.Files"];
    public static IReadOnlyList<string> SystemMonitorMenu { get; } = ["CenseqAdmin.SystemMonitor"];
    public static IReadOnlyList<string> SystemMonitorServer { get; } = ["CenseqAdmin.SystemMonitor.Server"];
    public static IReadOnlyList<string> SystemMonitorCache { get; } = ["CenseqAdmin.SystemMonitor.Cache"];
    public static IReadOnlyList<string> SystemMonitorCacheDelete { get; } = ["CenseqAdmin.SystemMonitor.Cache.Delete"];
    public static IReadOnlyList<string> SystemMonitorCacheClear { get; } = ["CenseqAdmin.SystemMonitor.Cache.Clear"];
    public static IReadOnlyList<string> FileProviderMenu { get; } = ["CenseqAdmin.FileProviders"];
    public static IReadOnlyList<string> FileProviderCreate { get; } = ["CenseqAdmin.FileProviders.Create"];
    public static IReadOnlyList<string> FileProviderUpdate { get; } = ["CenseqAdmin.FileProviders.Update"];
    public static IReadOnlyList<string> FileProviderDelete { get; } = ["CenseqAdmin.FileProviders.Delete"];
    public static IReadOnlyList<string> FileProviderSetDefault { get; } = ["CenseqAdmin.FileProviders.SetDefault"];

    public static IReadOnlyList<string> TenantMenu { get; } = ["TenantManagement.Tenants"];
    public static IReadOnlyList<string> TenantCreate { get; } = ["TenantManagement.Tenants.Create"];
    public static IReadOnlyList<string> TenantUpdate { get; } = ["TenantManagement.Tenants.Update"];
    public static IReadOnlyList<string> TenantDelete { get; } = ["TenantManagement.Tenants.Delete"];
    public static IReadOnlyList<string> TenantManageFeatures { get; } = ["TenantManagement.Tenants.ManageFeatures"];
    public static IReadOnlyList<string> TenantManageConnectionStrings { get; } = ["TenantManagement.Tenants.ManageConnectionStrings"];
    public static IReadOnlyList<string> TenantResetAdminPassword { get; } = ["TenantManagement.Tenants.ResetAdminPassword"];
    public static IReadOnlyList<string> TenantAdminMenu { get; } = ["TenantManagement.TenantAdmin"];
    public static IReadOnlyList<string> TenantAdminPermissions { get; } = ["TenantManagement.TenantAdmin.TenantPermissions"];
    public static IReadOnlyList<string> TenantAdminPermissionsUpdate { get; } = ["TenantManagement.TenantAdmin.TenantPermissions.Update"];

    public static IReadOnlyList<string> HostFeatureMenu { get; } = ["CenseqFeatureManagement.ManageHostFeatures"];

    public static IReadOnlyList<string> PermissionDefinitionMenu { get; } = ["PermissionManagement.DefinitionManagement"];
    public static IReadOnlyList<string> AuditLogMenu { get; } = ["AuditLogging.AuditLogs"];
    public static IReadOnlyList<string> AuditLogDelete { get; } = ["AuditLogging.AuditLogs.Delete"];
    public static IReadOnlyList<string> LogManagementMenu { get; } = Merge(AuditLogMenu, SecurityLogMenu);

    public static IReadOnlyList<string> SettingMenu { get; } = ["SettingManagement.Emailing", "SettingManagement.TimeZone"];
    public static IReadOnlyList<string> SettingDefinitions { get; } =
    [
        "SettingManagement.SettingDefinitions",
        "SettingManagement.SettingDefinitions.Create",
        "SettingManagement.SettingDefinitions.Update",
        "SettingManagement.SettingDefinitions.Delete"
    ];

    public static IReadOnlyList<string> LocalizationTexts { get; } = ["CenseqLocalizationManagement.Texts"];
    public static IReadOnlyList<string> LocalizationResources { get; } = ["CenseqLocalizationManagement.Resources"];
    public static IReadOnlyList<string> LocalizationCultures { get; } = ["CenseqLocalizationManagement.Cultures"];

    public static IReadOnlyList<string> OpenIddictApplicationMenu { get; } = ["OpenIddict.Applications"];
    public static IReadOnlyList<string> OpenIddictApplicationCreate { get; } = ["OpenIddict.Applications.Create"];
    public static IReadOnlyList<string> OpenIddictApplicationUpdate { get; } = ["OpenIddict.Applications.Update"];
    public static IReadOnlyList<string> OpenIddictApplicationDelete { get; } = ["OpenIddict.Applications.Delete"];

    public static IReadOnlyList<string> OpenIddictScopeMenu { get; } = ["OpenIddict.Scopes"];
    public static IReadOnlyList<string> OpenIddictScopeCreate { get; } = ["OpenIddict.Scopes.Create"];
    public static IReadOnlyList<string> OpenIddictScopeUpdate { get; } = ["OpenIddict.Scopes.Update"];
    public static IReadOnlyList<string> OpenIddictScopeDelete { get; } = ["OpenIddict.Scopes.Delete"];

    public static IReadOnlyList<string> HostAdminDefaults { get; } = Merge(
        UserMenu,
        UserCreate,
        UserUpdate,
        UserDelete,
        UserManageRoles,
        UserManagePermissions,
        OrganizationUnitMenu,
        OrganizationUnitCreate,
        OrganizationUnitUpdate,
        OrganizationUnitDelete,
        RoleMenu,
        RoleCreate,
        RoleUpdate,
        RoleDelete,
        RoleManagePermissions,
        ClaimTypeMenu,
        ClaimTypeCreate,
        ClaimTypeUpdate,
        ClaimTypeDelete,
        SessionMenu,
        SessionManage,
        SessionRevoke,
        TenantMenu,
        TenantCreate,
        TenantUpdate,
        TenantDelete,
        TenantManageFeatures,
        TenantManageConnectionStrings,
        TenantResetAdminPassword,
        TenantAdminMenu,
        TenantAdminPermissions,
        TenantAdminPermissionsUpdate,
        HostFeatureMenu,
        MenuManagementMenu,
        MenuManagementCreate,
        MenuManagementUpdate,
        MenuManagementDelete,
        MenuManagementManageStatus,
        MenuManagementManageOrder,
        FileManagementMenu,
        SystemMonitorMenu,
        SystemMonitorServer,
        SystemMonitorCache,
        SystemMonitorCacheDelete,
        SystemMonitorCacheClear,
        FileProviderMenu,
        FileProviderCreate,
        FileProviderUpdate,
        FileProviderDelete,
        FileProviderSetDefault,
        SettingMenu,
        SettingDefinitions,
        PermissionDefinitionMenu,
        LogManagementMenu,
        AuditLogMenu,
        AuditLogDelete,
        SecurityLogMenu,
        SecurityLogDelete,
        LocalizationTexts,
        LocalizationResources,
        LocalizationCultures,
        OpenIddictApplicationMenu,
        OpenIddictApplicationCreate,
        OpenIddictApplicationUpdate,
        OpenIddictApplicationDelete,
        OpenIddictScopeMenu,
        OpenIddictScopeCreate,
        OpenIddictScopeUpdate,
        OpenIddictScopeDelete
    );

    public static IReadOnlyList<string> TenantAdminDefaults { get; } = Merge(
        UserMenu,
        UserCreate,
        UserUpdate,
        UserDelete,
        UserManageRoles,
        UserManagePermissions,
        OrganizationUnitMenu,
        OrganizationUnitCreate,
        OrganizationUnitUpdate,
        OrganizationUnitDelete,
        RoleMenu,
        RoleCreate,
        RoleUpdate,
        RoleDelete,
        RoleManagePermissions,
        SessionMenu,
        SessionManage,
        SessionRevoke,
        MenuCopyFromHost
    );

    private static IReadOnlyList<string> Merge(params IReadOnlyList<string>[] groups)
    {
        return groups
            .SelectMany(static group => group)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }
}
