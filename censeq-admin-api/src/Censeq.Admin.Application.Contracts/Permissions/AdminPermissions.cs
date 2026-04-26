namespace Censeq.Admin.Permissions;

public static class AdminPermissions
{
    public const string GroupName = "CenseqAdmin";

    public static class Menus
    {
        public const string Default = GroupName + ".Menus";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManageStatus = Default + ".ManageStatus";
        public const string ManageOrder = Default + ".ManageOrder";
        public const string CopyFromHost = Default + ".CopyFromHost";
    }

    /// <summary>
    /// 平台侧对租户的授权范围管理。
    /// </summary>
    public static class TenantAdmin
    {
        public const string Default = GroupName + ".TenantAdmin";

        public static class TenantPermissions
        {
            public const string Default = TenantAdmin.Default + ".TenantPermissions";
            public const string Update  = Default + ".Update";
        }
    }
}
