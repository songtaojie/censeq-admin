using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 应用程序模块。
/// </summary>
[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(CenseqOpenIddictApplicationContractsModule),
    typeof(CenseqOpenIddictDomainModule)
)]
public class CenseqOpenIddictApplicationModule : AbpModule
{
    /// <summary>
    /// 配置 OpenIddict 应用程序模块 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 配置AutoMapper等
    }
}
