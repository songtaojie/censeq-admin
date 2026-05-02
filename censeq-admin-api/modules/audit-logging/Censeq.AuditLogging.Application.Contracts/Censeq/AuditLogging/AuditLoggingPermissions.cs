using Volo.Abp.Reflection;

namespace Censeq.AuditLogging;

public static class AuditLoggingPermissions
{
    public const string GroupName = "AuditLogging";

    public const string AuditLogs = GroupName + ".AuditLogs";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(AuditLoggingPermissions));
    }
}
