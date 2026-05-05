using Volo.Abp.Reflection;

namespace Censeq.SettingManagement;

public class SettingManagementPermissions
{
    public const string GroupName = "SettingManagement";

    public const string Emailing = GroupName + ".Emailing";

    public const string EmailingTest = Emailing + ".Test";

    public const string TimeZone = GroupName + ".TimeZone";

    public static class SettingDefinitions
    {
        public const string Default = GroupName + ".SettingDefinitions";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(SettingManagementPermissions));
    }
}
