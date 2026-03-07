using Censeq.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// 
/// </summary>
[DependsOn(
    typeof(StarshinePermissionManagementDomainModule),
    typeof(StarshinePermissionManagementApplicationContractsModule),
    typeof(StarshineDddApplicationModule)
    )]
public class StarshinePermissionManagementApplicationModule : AbpModule
{

}
