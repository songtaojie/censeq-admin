using Volo.Abp.Reflection;

namespace Censeq.Identity;

/// <summary>
/// 身份Permissions
/// </summary>
public static class IdentityPermissions
{
    public const string GroupName = "CenseqIdentity";

    /// <summary>
    /// Roles
    /// </summary>
    public static class Roles
    {
        public const string Default = GroupName + ".Roles";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
    }

    /// <summary>
    /// Users
    /// </summary>
    public static class Users
    {
        public const string Default = GroupName + ".Users";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManagePermissions = Default + ".ManagePermissions";
        public const string ManageRoles = Update + ".ManageRoles";
    }

    /// <summary>
    /// 用户查找
    /// </summary>
    public static class UserLookup
    {
        public const string Default = GroupName + ".UserLookup";
    }

    /// <summary>
    /// 组织Units
    /// </summary>
    public static class OrganizationUnits
    {
        public const string Default = GroupName + ".OrganizationUnits";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// Sessions
    /// </summary>
    public static class Sessions
    {
        public const string Default = GroupName + ".Sessions";
        public const string Manage = Default + ".Manage";
        public const string Revoke = Default + ".Revoke";
    }

    /// <summary>
    /// 登录日志
    /// </summary>
    public static class SecurityLogs
    {
        public const string Default = GroupName + ".SecurityLogs";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// 声明Types
    /// </summary>
    public static class ClaimTypes
    {
        public const string Default = GroupName + ".ClaimTypes";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    /// <summary>
    /// string[]
    /// </summary>
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(IdentityPermissions));
    }
}
