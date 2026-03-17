using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;
using Censeq.OpenIddict;

namespace Censeq.PermissionManagement.OpenIddict;

[DependsOn(
    typeof(CenseqOpenIddictDomainSharedModule),
    typeof(CenseqPermissionManagementDomainModule)
)]
public class CenseqPermissionManagementDomainOpenIddictModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<PermissionManagementOptions>(options =>
        {
            options.ManagementProviders.Add<ApplicationPermissionManagementProvider>();
            options.ProviderPolicies[ClientPermissionValueProvider.ProviderName] = "OpenIddictPro.Application.ManagePermissions";
        });
    }
}
