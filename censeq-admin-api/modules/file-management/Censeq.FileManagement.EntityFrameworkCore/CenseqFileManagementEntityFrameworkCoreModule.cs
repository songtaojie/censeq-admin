using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement.EntityFrameworkCore;

/// <summary>
/// 文件管理 Entity Framework Core 模块，提供文件管理实体的持久化能力。
/// </summary>
[DependsOn(
    typeof(AbpEntityFrameworkCoreModule),
    typeof(CenseqFileManagementDomainModule)
)]
public class CenseqFileManagementEntityFrameworkCoreModule : AbpModule
{
}
