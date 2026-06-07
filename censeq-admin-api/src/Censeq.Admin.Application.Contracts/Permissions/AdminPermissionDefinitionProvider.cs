using Censeq.Admin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.Admin.Permissions;

public class AdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var menusGroup = context.AddGroup(AdminPermissions.Menus.MenusGroupName, L("Permission:Menus"));
        var menusPermission = menusGroup.AddPermission(AdminPermissions.Menus.Default, L("Permission:Menus"));
        menusPermission.AddChild(AdminPermissions.Menus.Create, L("Permission:Create"));
        menusPermission.AddChild(AdminPermissions.Menus.Update, L("Permission:Edit"));
        menusPermission.AddChild(AdminPermissions.Menus.Delete, L("Permission:Delete"));
        menusPermission.AddChild(AdminPermissions.Menus.ManageStatus, L("Permission:ChangeStatus"));
        menusPermission.AddChild(AdminPermissions.Menus.ManageOrder, L("Permission:ManageOrder"));
        menusPermission.AddChild(AdminPermissions.Menus.CopyFromHost, L("Permission:CopyFromHost"));

        var myGroup = context.AddGroup(AdminPermissions.GroupName, L("Permission:CenseqAdmin"));

        var systemMonitorPermission = myGroup.AddPermission(AdminPermissions.SystemMonitor.Default, L("Permission:SystemMonitor"));
        systemMonitorPermission.AddChild(AdminPermissions.SystemMonitor.Server, L("Permission:SystemMonitorServer"));
        var cachePermission = systemMonitorPermission.AddChild(AdminPermissions.SystemMonitor.Cache, L("Permission:SystemMonitorCache"));
        cachePermission.AddChild(AdminPermissions.SystemMonitor.CacheDelete, L("Permission:Delete"));
        cachePermission.AddChild(AdminPermissions.SystemMonitor.CacheClear, L("Permission:Clear"));

        myGroup.AddPermission(AdminPermissions.Files.Default, L("Permission:Files"));

        var fileProvidersPermission = myGroup.AddPermission(AdminPermissions.FileProviders.Default, L("Permission:FileProviders"));
        fileProvidersPermission.AddChild(AdminPermissions.FileProviders.Create, L("Permission:Create"));
        fileProvidersPermission.AddChild(AdminPermissions.FileProviders.Update, L("Permission:Edit"));
        fileProvidersPermission.AddChild(AdminPermissions.FileProviders.Delete, L("Permission:Delete"));
        fileProvidersPermission.AddChild(AdminPermissions.FileProviders.SetDefault, L("Permission:SetDefault"));

        var tenantManagementGroup = context.GetGroup("TenantManagement");
        var tenantAdminPermission = tenantManagementGroup.AddPermission(AdminPermissions.TenantAdmin.Default, L("Permission:TenantAdmin"));
        var tenantPermsPermission = tenantAdminPermission.AddChild(AdminPermissions.TenantAdmin.TenantPermissions.Default, L("Permission:TenantPermissions"));
        tenantPermsPermission.AddChild(AdminPermissions.TenantAdmin.TenantPermissions.Update, L("Permission:Edit"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqAdminResource>(name);
    }
}
