using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.OpenIddict;

public static class CenseqOpenIddictDbProperties
{
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;

    public static string DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;

    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
