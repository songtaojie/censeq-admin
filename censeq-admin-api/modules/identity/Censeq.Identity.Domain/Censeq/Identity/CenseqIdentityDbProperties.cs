using Volo.Abp.Data;

namespace Censeq.Identity;

public static class CenseqIdentityDbProperties
{
    public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "CenseqIdentity";
}
