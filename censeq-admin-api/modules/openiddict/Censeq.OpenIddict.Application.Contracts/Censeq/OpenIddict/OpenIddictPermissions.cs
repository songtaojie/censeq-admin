namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 权限名称常量，集中声明权限名称。
/// </summary>
public static class OpenIddictPermissions
{
    /// <summary>
    /// 权限组名称。
    /// </summary>
    public const string GroupName = "OpenIddict";

    /// <summary>
    /// 应用程序。
    /// </summary>
    public static class Applications
    {
        /// <summary>
        /// 应用程序默认权限名称。
        /// </summary>
        public const string Default = GroupName + ".Applications";
        /// <summary>
        /// 创建权限名称。
        /// </summary>
        public const string Create = Default + ".Create";
        /// <summary>
        /// 更新权限名称。
        /// </summary>
        public const string Update = Default + ".Update";
        /// <summary>
        /// 删除权限名称。
        /// </summary>
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 作用域。
    /// </summary>
    public static class Scopes
    {
        /// <summary>
        /// 作用域默认权限名称。
        /// </summary>
        public const string Default = GroupName + ".Scopes";
        /// <summary>
        /// 创建权限名称。
        /// </summary>
        public const string Create = Default + ".Create";
        /// <summary>
        /// 更新权限名称。
        /// </summary>
        public const string Update = Default + ".Update";
        /// <summary>
        /// 删除权限名称。
        /// </summary>
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 授权。
    /// </summary>
    public static class Authorizations
    {
        /// <summary>
        /// 授权默认权限名称。
        /// </summary>
        public const string Default = GroupName + ".Authorizations";
        /// <summary>
        /// 删除权限名称。
        /// </summary>
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 令牌。
    /// </summary>
    public static class Tokens
    {
        /// <summary>
        /// 令牌默认权限名称。
        /// </summary>
        public const string Default = GroupName + ".Tokens";
        /// <summary>
        /// 删除权限名称。
        /// </summary>
        public const string Delete = Default + ".Delete";
    }
}
