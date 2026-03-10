using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace Censeq.SettingManagement;

[DependsOn(
    typeof(CenseqSettingManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule)
)]
public class CenseqSettingManagementApplicationContractsModule : AbpModule
{
}


