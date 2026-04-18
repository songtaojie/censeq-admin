using Volo.Abp.Reflection;

namespace Censeq.PermissionManagement;

public static class PermissionManagementPermissions
{
    public const string GroupName = "PermissionManagement";

    public const string DefinitionManagement = GroupName + ".DefinitionManagement";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(PermissionManagementPermissions));
    }
}