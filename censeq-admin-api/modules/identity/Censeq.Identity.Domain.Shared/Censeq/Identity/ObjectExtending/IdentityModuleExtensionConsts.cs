namespace Censeq.Identity.ObjectExtending;

/// <summary>
/// 身份模块扩展常量
/// </summary>
public static class IdentityModuleExtensionConsts
{
    /// <summary>
    /// 模块名称
    /// </summary>
    public const string ModuleName = "Identity";

    /// <summary>
    /// 实体名称常量
    /// </summary>
    public static class EntityNames
    {
        /// <summary>
        /// 用户实体
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// 角色实体
        /// </summary>
        public const string Role = "Role";

        /// <summary>
        /// 声明类型实体
        /// </summary>
        public const string ClaimType = "ClaimType";

        /// <summary>
        /// 组织单元实体
        /// </summary>
        public const string OrganizationUnit = "OrganizationUnit";
    }

    /// <summary>
    /// 配置名称常量
    /// </summary>
    public static class ConfigurationNames
    {
        /// <summary>
        /// 允许用户编辑
        /// </summary>
        public const string AllowUserToEdit = "AllowUserToEdit";
    }
}
