using Censeq.TenantManagement;

namespace Censeq.Admin.Permissions;

public static class AdminPermissions
{
    public const string GroupName = "CenseqAdmin";

    public static class Menus
    {
        public const string MenusGroupName = GroupName + ".Menus";
        public const string Default = GroupName + ".Menus";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManageStatus = Default + ".ManageStatus";
        public const string ManageOrder = Default + ".ManageOrder";
        public const string CopyFromHost = Default + ".CopyFromHost";
    }

    public static class Files
    {
        public const string Default = GroupName + ".Files";
    }

    public static class SystemMonitor
    {
        public const string Default = GroupName + ".SystemMonitor";
        public const string Server = Default + ".Server";
        public const string Cache = Default + ".Cache";
        public const string CacheDelete = Cache + ".Delete";
        public const string CacheClear = Cache + ".Clear";
    }

    public static class FileProviders
    {
        public const string Default = GroupName + ".FileProviders";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string SetDefault = Default + ".SetDefault";
    }

    /// <summary>
    /// 平台侧对租户的授权范围管理。
    /// </summary>
    public static class TenantAdmin
    {
        public const string Default = TenantManagementPermissions.GroupName + ".TenantAdmin";

        public static class TenantPermissions
        {
            public const string Default = TenantAdmin.Default + ".TenantPermissions";
            public const string Update  = Default + ".Update";
        }
    }
}
