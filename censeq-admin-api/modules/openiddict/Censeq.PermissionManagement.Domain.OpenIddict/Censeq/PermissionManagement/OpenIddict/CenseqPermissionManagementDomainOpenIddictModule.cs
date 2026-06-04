using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;
using Censeq.OpenIddict;

namespace Censeq.PermissionManagement.OpenIddict;

/// <summary>
/// 权限Management领域OpenIddict模块。
/// </summary>
[DependsOn(
    typeof(CenseqOpenIddictDomainSharedModule),
    typeof(CenseqPermissionManagementDomainModule)
)]
public class CenseqPermissionManagementDomainOpenIddictModule : AbpModule
{
    /// <summary>
    /// 配置 权限Management领域OpenIddict模块 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<PermissionManagementOptions>(options =>
        {
            options.ManagementProviders.Add<ApplicationPermissionManagementProvider>();
            options.ProviderPolicies[ClientPermissionValueProvider.ProviderName] = "OpenIddictPro.Application.ManagePermissions";
        });
    }
}
