using Censeq.OpenIddict.Localization;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Reflection;
using Volo.Abp.Threading;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 应用程序契约模块。
/// </summary>
[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule),
    typeof(CenseqOpenIddictDomainSharedModule)
)]
public class CenseqOpenIddictApplicationContractsModule : AbpModule
{
    /// <summary>
    /// 预配置 OpenIddict 应用程序契约模块 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        OpenIddictDtoExtensions.Configure();
    }
}

/// <summary>
/// OpenIddict DTO 扩展方法。
/// </summary>
public static class OpenIddictDtoExtensions
{
    /// <summary>
    /// 一次性执行器。
    /// </summary>
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    /// <summary>
    /// 配置 OpenIddict DTO 扩展映射。
    /// </summary>
    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            // 配置DTO扩展
        });
    }
}
