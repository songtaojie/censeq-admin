using Volo.Abp.Data;

namespace Censeq.TenantManagement;

public static class CenseqTenantManagementDbProperties
{
    public static string? DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "AbpTenantManagement";
}
