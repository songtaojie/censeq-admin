using Censeq.PermissionManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理模块权限定义提供者。
/// </summary>
public class PermissionManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    /// <summary>
    /// 定义权限管理模块的权限项。
    /// </summary>
    /// <param name="context">权限定义上下文。</param>
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            PermissionManagementPermissions.GroupName,
            L("Permission:PermissionManagement"));

        group.AddPermission(
            PermissionManagementPermissions.DefinitionManagement,
            L("Permission:DefinitionManagement"));
    }

    /// <summary>
    /// 创建权限管理模块的本地化字符串。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <returns>本地化字符串。</returns>
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqPermissionManagementResource>(name);
    }
}