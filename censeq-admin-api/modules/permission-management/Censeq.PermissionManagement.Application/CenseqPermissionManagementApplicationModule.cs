using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.PermissionManagement;

/// <summary>
/// 
/// </summary>
[DependsOn(
    typeof(CenseqPermissionManagementDomainModule),
    typeof(CenseqPermissionManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule)
    )]
public class CenseqPermissionManagementApplicationModule : AbpModule
{

}
