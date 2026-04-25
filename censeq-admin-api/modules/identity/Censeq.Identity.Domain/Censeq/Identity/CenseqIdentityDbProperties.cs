using Censeq.Framework.EntityFrameworkCore;
using Volo.Abp.Data;

namespace Censeq.Identity;

/// <summary>
/// Censeq身份数据库Properties
/// </summary>
public static class CenseqIdentityDbProperties
{
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;

    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
