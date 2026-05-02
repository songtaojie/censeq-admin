using Censeq.AuditLogging.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.AuditLogging;

public class AuditLoggingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            AuditLoggingPermissions.GroupName,
            L("Permission:AuditLogging"));

        group.AddPermission(
            AuditLoggingPermissions.AuditLogs,
            L("Permission:AuditLogs"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqAuditLoggingResource>(name);
    }
}
