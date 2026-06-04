using Censeq.AuditLogging.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志权限定义提供者，用于定义模块权限项。
/// </summary>
public class AuditLoggingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    /// <summary>
    /// 定义审计日志模块权限项。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            AuditLoggingPermissions.GroupName,
            L("Permission:AuditLogging"));

        group.AddPermission(
            AuditLoggingPermissions.AuditLogs,
            L("Permission:AuditLogs"));
    }

    /// <summary>
    /// 创建审计日志模块的本地化字符串。
    /// </summary>
    /// <param name="name">name。</param>
    /// <returns>本地化字符串。</returns>
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqAuditLoggingResource>(name);
    }
}
