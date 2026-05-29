using Volo.Abp.Modularity;
using Volo.Abp.Validation;

namespace Censeq.FileManagement;

/// <summary>
/// 文件管理领域共享模块，注册校验等跨层共享能力。
/// </summary>
[DependsOn(typeof(AbpValidationModule))]
public class CenseqFileManagementDomainSharedModule : AbpModule
{
}
