using Censeq.PermissionManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.PermissionManagement;

public class PermissionManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            PermissionManagementPermissions.GroupName,
            L("Permission:PermissionManagement"));

        group.AddPermission(
            PermissionManagementPermissions.DefinitionManagement,
            L("Permission:DefinitionManagement"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqPermissionManagementResource>(name);
    }
}