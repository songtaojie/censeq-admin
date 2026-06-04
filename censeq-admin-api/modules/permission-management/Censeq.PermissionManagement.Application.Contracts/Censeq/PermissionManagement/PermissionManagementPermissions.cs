using Volo.Abp.Reflection;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理模块权限名称常量。
/// </summary>
public static class PermissionManagementPermissions
{
    /// <summary>
    /// 权限管理权限组名称。
    /// </summary>
    public const string GroupName = "PermissionManagement";

    /// <summary>
    /// 权限定义管理权限名称。
    /// </summary>
    public const string DefinitionManagement = GroupName + ".DefinitionManagement";

    /// <summary>
    /// 获取权限管理模块定义的全部权限名称。
    /// </summary>
    /// <returns>权限名称集合。</returns>
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(PermissionManagementPermissions));
    }
}