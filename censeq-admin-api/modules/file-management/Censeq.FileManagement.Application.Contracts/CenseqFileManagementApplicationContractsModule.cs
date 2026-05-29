using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

/// <summary>
/// 文件管理应用契约模块，提供 DTO、应用服务接口和授权依赖。
/// </summary>
[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule),
    typeof(CenseqFileManagementDomainSharedModule)
)]
public class CenseqFileManagementApplicationContractsModule : AbpModule
{
}
