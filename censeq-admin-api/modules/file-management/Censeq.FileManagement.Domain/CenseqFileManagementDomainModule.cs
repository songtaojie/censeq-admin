using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(CenseqFileManagementDomainSharedModule)
)]
public class CenseqFileManagementDomainModule : AbpModule
{
}
