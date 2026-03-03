using Starshine.Admin.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Starshine.Admin.FeatureManagement;

public class FeaturePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var featureManagementGroup = context.AddGroup(
            FeatureManagementPermissions.GroupName,
            L("Permission:FeatureManagement"));

        featureManagementGroup.AddPermission(
            FeatureManagementPermissions.ManageHostFeatures,
            L("Permission:FeatureManagement.ManageHostFeatures"),
            multiTenancySide: MultiTenancySides.Host);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<StarshineFeatureManagementResource>(name);
    }
}
