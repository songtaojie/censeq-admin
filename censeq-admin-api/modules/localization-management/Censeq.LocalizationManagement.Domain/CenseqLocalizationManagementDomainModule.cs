using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace Censeq.LocalizationManagement;

[DependsOn(
    typeof(CenseqLocalizationManagementDomainSharedModule),
    typeof(AbpCachingModule)
)]
public class CenseqLocalizationManagementDomainModule : AbpModule
{
}
