using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Censeq.Identity;

namespace Censeq.PermissionManagement.Identity;

[DependsOn(
    typeof(CenseqIdentityDomainSharedModule),
    typeof(CenseqPermissionManagementDomainModule),
    typeof(AbpUsersAbstractionModule)
)]
/// <summary>
/// Censeq权限管理领域身份模块
/// </summary>
public class CenseqPermissionManagementDomainIdentityModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<PermissionManagementOptions>(options =>
        {
            options.ManagementProviders.Add<UserPermissionManagementProvider>();
            options.ManagementProviders.Add<RolePermissionManagementProvider>();

            //TODO: Can we prevent duplication of permission names without breaking the design and making the system complicated
            options.ProviderPolicies[UserPermissionValueProvider.ProviderName] = "CenseqIdentity.Users.ManagePermissions";
            options.ProviderPolicies[RolePermissionValueProvider.ProviderName] = "CenseqIdentity.Roles.ManagePermissions";
        });
    }
}
