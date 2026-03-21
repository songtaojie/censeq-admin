using Censeq.Framework.EntityFrameworkCore;
using Volo.Abp.Data;

namespace Censeq.SettingManagement;

public static class CenseqSettingManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;

    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
