using Censeq.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.Abp.Users.EntityFrameworkCore;

/// <summary>
/// 用户ef模块
/// </summary>
[DependsOn(
    typeof(CenseqUsersDomainModule),
    typeof(CenseqEntityFrameworkCoreModule)
    )]
public class CenseqUsersEntityFrameworkCoreModule : AbpModule
{

}
