using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Censeq.Identity.Localization;

namespace Censeq.Identity;

public class IdentityPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var identityGroup = context.AddGroup(IdentityPermissions.GroupName, L("Permission:IdentityManagement"));

        var rolesPermission = identityGroup.AddPermission(IdentityPermissions.Roles.Default, L("Permission:RoleManagement"));
        rolesPermission.AddChild(IdentityPermissions.Roles.Create, L("Permission:Create"));
        rolesPermission.AddChild(IdentityPermissions.Roles.Update, L("Permission:Edit"));
        rolesPermission.AddChild(IdentityPermissions.Roles.Delete, L("Permission:Delete"));
        rolesPermission.AddChild(IdentityPermissions.Roles.ManagePermissions, L("Permission:ChangePermissions"));

        var usersPermission = identityGroup.AddPermission(IdentityPermissions.Users.Default, L("Permission:UserManagement"));
        usersPermission.AddChild(IdentityPermissions.Users.Create, L("Permission:Create"));
        var editPermission = usersPermission.AddChild(IdentityPermissions.Users.Update, L("Permission:Edit"));
        editPermission.AddChild(IdentityPermissions.Users.ManageRoles, L("Permission:ManageRoles"));
        usersPermission.AddChild(IdentityPermissions.Users.Delete, L("Permission:Delete"));
        usersPermission.AddChild(IdentityPermissions.Users.ManagePermissions, L("Permission:ChangePermissions"));

        var organizationUnitsPermission = identityGroup.AddPermission(IdentityPermissions.OrganizationUnits.Default, L("Permission:OrganizationUnitManagement"));
        organizationUnitsPermission.AddChild(IdentityPermissions.OrganizationUnits.Create, L("Permission:Create"));
        organizationUnitsPermission.AddChild(IdentityPermissions.OrganizationUnits.Update, L("Permission:Edit"));
        organizationUnitsPermission.AddChild(IdentityPermissions.OrganizationUnits.Delete, L("Permission:Delete"));

        var sessionsPermission = identityGroup.AddPermission(IdentityPermissions.Sessions.Default, L("Permission:SessionManagement"));
        sessionsPermission.AddChild(IdentityPermissions.Sessions.Manage, L("Permission:Manage"));
        sessionsPermission.AddChild(IdentityPermissions.Sessions.Revoke, L("Permission:Revoke"));

        var claimTypesPermission = identityGroup.AddPermission(IdentityPermissions.ClaimTypes.Default, L("Permission:ClaimTypeManagement"));
        claimTypesPermission.AddChild(IdentityPermissions.ClaimTypes.Create, L("Permission:Create"));
        claimTypesPermission.AddChild(IdentityPermissions.ClaimTypes.Update, L("Permission:Edit"));
        claimTypesPermission.AddChild(IdentityPermissions.ClaimTypes.Delete, L("Permission:Delete"));

        identityGroup
            .AddPermission(IdentityPermissions.UserLookup.Default, L("Permission:UserLookup"))
            .WithProviders(ClientPermissionValueProvider.ProviderName);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IdentityResource>(name);
    }
}
