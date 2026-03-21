using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.PermissionManagement;

public static class CenseqPermissionManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;
    public static string? DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;
    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
