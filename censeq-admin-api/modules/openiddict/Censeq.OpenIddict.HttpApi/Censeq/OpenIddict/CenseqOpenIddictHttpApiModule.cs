using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict HTTP API 模块。
/// </summary>
[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(CenseqOpenIddictApplicationModule)
)]
public class CenseqOpenIddictHttpApiModule : AbpModule
{
    /// <summary>
    /// 配置 OpenIddict HTTP API 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 配置API
    }
}
