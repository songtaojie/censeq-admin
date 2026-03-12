using Volo.Abp.Data;

namespace Censeq.FeatureManagement;

public static class CenseqFeatureManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

    public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "CenseqFeatureManagement";
}
