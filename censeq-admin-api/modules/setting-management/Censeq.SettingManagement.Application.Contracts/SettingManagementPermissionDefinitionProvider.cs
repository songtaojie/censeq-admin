using Censeq.SettingManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Censeq.SettingManagement;

public class SettingManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var moduleGroup = context.AddGroup(SettingManagementPermissions.GroupName, L("Permission:SettingManagement"));

        var emailPermission = moduleGroup
            .AddPermission(SettingManagementPermissions.Emailing, L("Permission:Emailing"));
        emailPermission.StateCheckers.Add(new AllowChangingEmailSettingsFeatureSimpleStateChecker());

        emailPermission.AddChild(SettingManagementPermissions.EmailingTest, L("Permission:EmailingTest"));

        moduleGroup.AddPermission(SettingManagementPermissions.TimeZone, L("Permission:TimeZone"));

        var settingDefinitionPermission = moduleGroup.AddPermission(SettingManagementPermissions.SettingDefinitions.Default, L("Permission:SettingDefinitions"));
        settingDefinitionPermission.AddChild(SettingManagementPermissions.SettingDefinitions.Create, L("Permission:Create"));
        settingDefinitionPermission.AddChild(SettingManagementPermissions.SettingDefinitions.Update, L("Permission:Update"));
        settingDefinitionPermission.AddChild(SettingManagementPermissions.SettingDefinitions.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqSettingManagementResource>(name);
    }
}
