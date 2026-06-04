using Volo.Abp.Reflection;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志权限名称常量，集中声明权限名称。
/// </summary>
public static class AuditLoggingPermissions
{
    /// <summary>
    /// 权限分组名称。
    /// </summary>
    public const string GroupName = "AuditLogging";

    /// <summary>
    /// AuditLogs 常量。
    /// </summary>
    public const string AuditLogs = GroupName + ".AuditLogs";

    /// <summary>
    /// 获取全部审计日志模块扩展配置。
    /// </summary>
    /// <returns>模块扩展配置集合。</returns>
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AuditLoggingPermissions));
    }
}
