using Censeq.OpenIddict.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 权限定义提供者，用于定义模块权限项。
/// </summary>
public class OpenIddictPermissionDefinitionProvider : PermissionDefinitionProvider
{
    /// <summary>
    /// 定义 OpenIddict 模块的权限项。
    /// </summary>
    /// <param name="context">权限定义上下文。</param>
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

    /// <summary>
    /// 创建 OpenIddict 模块的本地化字符串。
    /// </summary>
    /// <param name="name">name。</param>
    /// <returns>操作结果。</returns>
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqOpenIddictResource>(name);
    }
}
