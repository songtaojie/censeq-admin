using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Json;
using Volo.Abp.Modularity;

namespace Censeq.LocalizationManagement;

[DependsOn(
    typeof(CenseqLocalizationManagementDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpJsonModule)
)]
public class CenseqLocalizationManagementApplicationContractsModule : AbpModule
{
}
