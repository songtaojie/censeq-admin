using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Users;
using Volo.Abp.Threading;
using Censeq.PermissionManagement;
using Censeq.Identity.ObjectExtending;

namespace Censeq.Identity;

/// <summary>
/// 身份认证应用契约模块
/// </summary>
[DependsOn(
    typeof(CenseqIdentityDomainSharedModule),
    typeof(AbpUsersAbstractionModule),
    typeof(AbpAuthorizationModule),
    typeof(CenseqPermissionManagementApplicationContractsModule)
    )]
public class CenseqIdentityApplicationContractsModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }

    /// <summary>
    /// 后配置服务
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                IdentityModuleExtensionConsts.ModuleName,
                IdentityModuleExtensionConsts.EntityNames.Role,
                getApiTypes: new[] { typeof(IdentityRoleDto) },
                createApiTypes: new[] { typeof(IdentityRoleCreateDto) },
                updateApiTypes: new[] { typeof(IdentityRoleUpdateDto) }
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                IdentityModuleExtensionConsts.ModuleName,
                IdentityModuleExtensionConsts.EntityNames.User,
                getApiTypes: new[] { typeof(IdentityUserDto) },
                createApiTypes: new[] { typeof(IdentityUserCreateDto) },
                updateApiTypes: new[] { typeof(IdentityUserUpdateDto) }
            );
        });
    }
}
