using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Censeq.Identity.Localization;

namespace Censeq.Identity;

[DependsOn(
    typeof(AbpUsersDomainSharedModule),
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule)
    )]
/// <summary>
/// Censeq 身份领域共享模块
/// </summary>
public class CenseqIdentityDomainSharedModule : AbpModule
{
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqIdentityDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<IdentityResource>("zh-Hans")
                .AddBaseTypes(
                    typeof(AbpValidationResource)
                )
                // 须与 csproj 中 EmbeddedResource 路径一致：Censeq/Identity/Localization/*.json
                .AddVirtualJson("/Censeq/Identity/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Censeq.Identity", typeof(IdentityResource));
        });
    }
}
