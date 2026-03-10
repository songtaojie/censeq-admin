using Volo.Abp.Application;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace Censeq.SettingManagement;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(CenseqSettingManagementDomainModule),
    typeof(CenseqSettingManagementApplicationContractsModule),
    typeof(AbpEmailingModule),
    typeof(AbpTimingModule),
    typeof(AbpUsersAbstractionModule)
)]
public class CenseqSettingManagementApplicationModule : AbpModule
{
}
