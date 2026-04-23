using Censeq.Admin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.Admin.Permissions;

public class AdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AdminPermissions.GroupName, L("Permission:CenseqAdmin"));

        var menusPermission = myGroup.AddPermission(AdminPermissions.Menus.Default, L("Permission:Menus"));
        menusPermission.AddChild(AdminPermissions.Menus.Create, L("Permission:Create"));
        menusPermission.AddChild(AdminPermissions.Menus.Update, L("Permission:Edit"));
        menusPermission.AddChild(AdminPermissions.Menus.Delete, L("Permission:Delete"));
        menusPermission.AddChild(AdminPermissions.Menus.ManageStatus, L("Permission:ChangeStatus"));
        menusPermission.AddChild(AdminPermissions.Menus.ManageOrder, L("Permission:ManageOrder"));
        menusPermission.AddChild(AdminPermissions.Menus.CopyFromHost, L("Permission:CopyFromHost"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqAdminResource>(name);
    }
}
