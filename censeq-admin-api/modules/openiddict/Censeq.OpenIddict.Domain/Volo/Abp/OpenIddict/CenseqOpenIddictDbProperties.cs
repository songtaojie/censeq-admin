using Volo.Abp.Data;

namespace Censeq.OpenIddict;

public static class CenseqOpenIddictDbProperties
{
    public static string DbTablePrefix { get; set; } = "OpenIddict";

    public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "CenseqOpenIddict";
}
