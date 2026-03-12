using Volo.Abp.Data;

namespace Censeq.PermissionManagement;

public static class CenseqPermissionManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = "Censeq";
    public static string? DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;
    public const string ConnectionStringName = "CenseqPermissionManagement";
}
