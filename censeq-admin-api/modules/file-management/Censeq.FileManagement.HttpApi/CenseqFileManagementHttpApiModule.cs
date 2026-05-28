using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(CenseqFileManagementApplicationModule)
)]
public class CenseqFileManagementHttpApiModule : AbpModule
{
}
