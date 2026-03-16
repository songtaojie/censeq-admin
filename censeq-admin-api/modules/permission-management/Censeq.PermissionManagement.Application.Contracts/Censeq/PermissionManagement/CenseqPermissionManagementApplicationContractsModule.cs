using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理应用契约
/// </summary>
[DependsOn(typeof(AbpDddApplicationContractsModule))]
[DependsOn(typeof(CenseqPermissionManagementDomainSharedModule))]
[DependsOn(typeof(AbpAuthorizationAbstractionsModule))]
public class CenseqPermissionManagementApplicationContractsModule : AbpModule
{

}
