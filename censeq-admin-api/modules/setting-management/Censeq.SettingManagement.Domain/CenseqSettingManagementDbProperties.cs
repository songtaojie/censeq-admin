using Volo.Abp.Data;

namespace Censeq.SettingManagement;

public static class CenseqSettingManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "AbpSettingManagement";
}
