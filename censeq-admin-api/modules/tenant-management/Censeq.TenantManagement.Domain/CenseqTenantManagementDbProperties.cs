using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.TenantManagement;

public static class CenseqTenantManagementDbProperties
{
    public static string? DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;

    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
