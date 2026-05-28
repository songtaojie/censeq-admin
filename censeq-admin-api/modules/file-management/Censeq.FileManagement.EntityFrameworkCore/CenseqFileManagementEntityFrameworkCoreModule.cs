using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement.EntityFrameworkCore;

[DependsOn(
    typeof(AbpEntityFrameworkCoreModule),
    typeof(CenseqFileManagementDomainModule)
)]
public class CenseqFileManagementEntityFrameworkCoreModule : AbpModule
{
}
