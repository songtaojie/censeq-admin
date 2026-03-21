using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.AuditLogging;

public static class CenseqAuditLoggingDbProperties
{
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;

    public static string? DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;

    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
