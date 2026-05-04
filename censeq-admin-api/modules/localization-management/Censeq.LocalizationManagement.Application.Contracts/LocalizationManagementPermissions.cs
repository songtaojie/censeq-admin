using Volo.Abp.Reflection;

namespace Censeq.LocalizationManagement;

public class LocalizationManagementPermissions
{
    public const string GroupName = "CenseqLocalizationManagement";

    public static class Resources
    {
        public const string Default = GroupName + ".Resources";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Cultures
    {
        public const string Default = GroupName + ".Cultures";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class Texts
    {
        public const string Default = GroupName + ".Texts";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(LocalizationManagementPermissions));
    }
}
