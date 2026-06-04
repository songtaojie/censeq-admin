using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理模块数据库配置属性。
/// </summary>
public static class CenseqPermissionManagementDbProperties
{
    /// <summary>
    /// 权限管理数据库表名前缀。
    /// </summary>
    public static string DbTablePrefix { get; set; } = CenseqCommonDbProperties.DbTablePrefix;
    /// <summary>
    /// 权限管理数据库架构名称。
    /// </summary>
    public static string? DbSchema { get; set; } = CenseqCommonDbProperties.DbSchema;
    /// <summary>
    /// ConnectionStringName 常量。
    /// </summary>
    public const string ConnectionStringName = CenseqCommonDbProperties.ConnectionStringName;
}
