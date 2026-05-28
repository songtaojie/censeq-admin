using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule),
    typeof(CenseqFileManagementDomainSharedModule)
)]
public class CenseqFileManagementApplicationContractsModule : AbpModule
{
}
