namespace Censeq.Admin.Menus;

/// <summary>
/// 菜单作用域：区分平台专属菜单和可分配给租户的菜单。
/// </summary>
public enum MenuScope : byte
{
    /// <summary>
    /// 平台专属菜单，仅平台管理员（host 用户）可见。
    /// 例如：租户管理、权限定义、审计日志、特性管理等。
    /// </summary>
    Platform = 1,

    /// <summary>
    /// 租户菜单，可分配给租户使用。平台统一定义，租户通过权限范围（TenantPermissionGrant）控制可见性。
    /// 例如：角色管理、用户管理、部门管理、应用设置等。
    /// </summary>
    Tenant = 2,
}
