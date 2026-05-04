using Censeq.LocalizationManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Censeq.LocalizationManagement;

public class LocalizationManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            LocalizationManagementPermissions.GroupName,
            L("Permission:LocalizationManagement"));

        var resources = group.AddPermission(
            LocalizationManagementPermissions.Resources.Default,
            L("Permission:LocalizationManagement.Resources"),
            multiTenancySide: MultiTenancySides.Host);
        resources.AddChild(LocalizationManagementPermissions.Resources.Create, L("Permission:LocalizationManagement.Resources.Create"), multiTenancySide: MultiTenancySides.Host);
        resources.AddChild(LocalizationManagementPermissions.Resources.Update, L("Permission:LocalizationManagement.Resources.Update"), multiTenancySide: MultiTenancySides.Host);
        resources.AddChild(LocalizationManagementPermissions.Resources.Delete, L("Permission:LocalizationManagement.Resources.Delete"), multiTenancySide: MultiTenancySides.Host);

        var cultures = group.AddPermission(
            LocalizationManagementPermissions.Cultures.Default,
            L("Permission:LocalizationManagement.Cultures"));
        cultures.AddChild(LocalizationManagementPermissions.Cultures.Create, L("Permission:LocalizationManagement.Cultures.Create"));
        cultures.AddChild(LocalizationManagementPermissions.Cultures.Update, L("Permission:LocalizationManagement.Cultures.Update"));
        cultures.AddChild(LocalizationManagementPermissions.Cultures.Delete, L("Permission:LocalizationManagement.Cultures.Delete"));

        var texts = group.AddPermission(
            LocalizationManagementPermissions.Texts.Default,
            L("Permission:LocalizationManagement.Texts"));
        texts.AddChild(LocalizationManagementPermissions.Texts.Create, L("Permission:LocalizationManagement.Texts.Create"));
        texts.AddChild(LocalizationManagementPermissions.Texts.Update, L("Permission:LocalizationManagement.Texts.Update"));
        texts.AddChild(LocalizationManagementPermissions.Texts.Delete, L("Permission:LocalizationManagement.Texts.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqLocalizationManagementResource>(name);
    }
}
