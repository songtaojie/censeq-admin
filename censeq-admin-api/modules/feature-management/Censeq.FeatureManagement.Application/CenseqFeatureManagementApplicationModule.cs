using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.FeatureManagement;

[DependsOn(
    typeof(CenseqFeatureManagementDomainModule),
    typeof(CenseqFeatureManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule)
    )]
public class CenseqFeatureManagementApplicationModule : AbpModule
{

}
