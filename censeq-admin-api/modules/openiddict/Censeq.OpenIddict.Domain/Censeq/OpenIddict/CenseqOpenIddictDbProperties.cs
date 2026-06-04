using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 数据库属性。
/// </summary>
public static class CenseqOpenIddictDbProperties
{
    /// <summary>
    /// 数据库表前缀。
    /// </summary>
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;

    /// <summary>
    /// 数据库架构。
    /// </summary>
    public static string DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;

    /// <summary>
    /// 连接字符串名称常量。
    /// </summary>
    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
