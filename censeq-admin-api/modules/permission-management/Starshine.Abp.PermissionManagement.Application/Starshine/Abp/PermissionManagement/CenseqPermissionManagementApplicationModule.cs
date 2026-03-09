using Censeq.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// 
/// </summary>
[DependsOn(
    typeof(CenseqPermissionManagementDomainModule),
    typeof(CenseqPermissionManagementApplicationContractsModule),
    typeof(CenseqDddApplicationModule)
    )]
public class CenseqPermissionManagementApplicationModule : AbpModule
{

}
