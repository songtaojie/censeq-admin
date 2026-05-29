using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

/// <summary>
/// 文件管理领域模块，承载文件记录和存储提供器等领域对象。
/// </summary>
[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(CenseqFileManagementDomainSharedModule)
)]
public class CenseqFileManagementDomainModule : AbpModule
{
}
