using Volo.Abp.Data;

namespace Censeq.AuditLogging;

public static class CenseqAuditLoggingDbProperties
{
    public static string DbTablePrefix { get; set; } = "Censeq";

    public static string? DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "CenseqAuditLogging";
}
