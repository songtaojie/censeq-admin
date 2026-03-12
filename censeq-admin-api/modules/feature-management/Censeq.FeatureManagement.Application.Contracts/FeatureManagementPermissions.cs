using Volo.Abp.Reflection;

namespace Censeq.FeatureManagement;

public class FeatureManagementPermissions
{
    public const string GroupName = "CenseqFeatureManagement";

    public const string ManageHostFeatures = GroupName + ".ManageHostFeatures";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(FeatureManagementPermissions));
    }
}
