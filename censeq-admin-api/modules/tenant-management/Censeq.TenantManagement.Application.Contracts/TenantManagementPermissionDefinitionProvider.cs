using Censeq.TenantManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.TenantManagement;

/// <summary>
/// 注册租户管理权限定义；未注册时 <c>[Authorize(TenantManagementPermissions...)]</c> 无法解析对应 AuthorizationPolicy。
/// </summary>
public class TenantManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var tenantManagementGroup = context.AddGroup(TenantManagementPermissions.GroupName, L("Permission:TenantManagement"));

        var tenantsPermission = tenantManagementGroup.AddPermission(
            TenantManagementPermissions.Tenants.Default,
            L("Tenants"));
        tenantsPermission.AddChild(TenantManagementPermissions.Tenants.Create, L("Permission:Create"));
        tenantsPermission.AddChild(TenantManagementPermissions.Tenants.Update, L("Permission:Edit"));
        tenantsPermission.AddChild(TenantManagementPermissions.Tenants.Delete, L("Permission:Delete"));
        tenantsPermission.AddChild(TenantManagementPermissions.Tenants.ManageFeatures, L("Permission:ManageFeatures"));
        tenantsPermission.AddChild(TenantManagementPermissions.Tenants.ManageConnectionStrings, L("Permission:ManageConnectionStrings"));
        tenantsPermission.AddChild(TenantManagementPermissions.Tenants.ResetAdminPassword, L("Permission:ResetAdminPassword"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqTenantManagementResource>(name);
    }
}
