using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.OpenIddict;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(CenseqOpenIddictApplicationContractsModule),
    typeof(CenseqOpenIddictDomainModule)
)]
public class CenseqOpenIddictApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 配置AutoMapper等
    }
}
