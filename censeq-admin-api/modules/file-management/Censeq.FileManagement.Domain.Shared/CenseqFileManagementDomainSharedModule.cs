using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace Censeq.FileManagement;

[DependsOn(typeof(AbpValidationModule))]
public class CenseqFileManagementDomainSharedModule : AbpModule
{
}
