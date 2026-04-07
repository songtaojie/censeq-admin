using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Censeq.OpenIddict;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(CenseqOpenIddictApplicationModule)
)]
public class CenseqOpenIddictHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 配置API
    }
}
