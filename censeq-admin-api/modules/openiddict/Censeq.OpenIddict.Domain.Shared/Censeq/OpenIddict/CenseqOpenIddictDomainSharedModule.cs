using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Censeq.OpenIddict.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 领域共享模块。
/// </summary>
[DependsOn(
    typeof(AbpValidationModule)
)]
public class CenseqOpenIddictDomainSharedModule : AbpModule
{
    /// <summary>
    /// 配置 OpenIddict 领域共享模块 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqOpenIddictDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<CenseqOpenIddictResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("Volo/Abp/OpenIddict/Localization/OpenIddict")
                .AddVirtualJson("/Censeq/OpenIddict/Localization/OpenIddict");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("OpenIddict", typeof(CenseqOpenIddictResource));
        });
    }
}
