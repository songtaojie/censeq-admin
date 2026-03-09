using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Censeq.Abp.Users;

/// <summary>
/// 用户领域模块
/// </summary>
[DependsOn(
    typeof(CenseqUsersDomainSharedModule),
    typeof(CenseqUsersAbstractionModule),
    typeof(AbpDddDomainModule)
    )]
public class CenseqUsersDomainModule : AbpModule
{

}
