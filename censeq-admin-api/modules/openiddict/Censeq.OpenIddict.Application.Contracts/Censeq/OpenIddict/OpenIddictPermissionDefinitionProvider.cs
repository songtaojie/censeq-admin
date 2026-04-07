using Censeq.OpenIddict.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.OpenIddict;

public class OpenIddictPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var openIddictGroup = context.AddGroup(OpenIddictPermissions.GroupName, L("Permission:OpenIddict"));

        // 应用管理权限
        var applicationsPermission = openIddictGroup.AddPermission(OpenIddictPermissions.Applications.Default, L("Permission:Applications"));
        applicationsPermission.AddChild(OpenIddictPermissions.Applications.Create, L("Permission:Create"));
        applicationsPermission.AddChild(OpenIddictPermissions.Applications.Update, L("Permission:Update"));
        applicationsPermission.AddChild(OpenIddictPermissions.Applications.Delete, L("Permission:Delete"));

        // 作用域管理权限
        var scopesPermission = openIddictGroup.AddPermission(OpenIddictPermissions.Scopes.Default, L("Permission:Scopes"));
        scopesPermission.AddChild(OpenIddictPermissions.Scopes.Create, L("Permission:Create"));
        scopesPermission.AddChild(OpenIddictPermissions.Scopes.Update, L("Permission:Update"));
        scopesPermission.AddChild(OpenIddictPermissions.Scopes.Delete, L("Permission:Delete"));

        // 授权管理权限
        var authorizationsPermission = openIddictGroup.AddPermission(OpenIddictPermissions.Authorizations.Default, L("Permission:Authorizations"));
        authorizationsPermission.AddChild(OpenIddictPermissions.Authorizations.Delete, L("Permission:Delete"));

        // 令牌管理权限
        var tokensPermission = openIddictGroup.AddPermission(OpenIddictPermissions.Tokens.Default, L("Permission:Tokens"));
        tokensPermission.AddChild(OpenIddictPermissions.Tokens.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqOpenIddictResource>(name);
    }
}
