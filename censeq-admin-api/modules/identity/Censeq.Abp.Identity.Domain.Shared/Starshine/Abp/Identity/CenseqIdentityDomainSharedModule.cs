using Microsoft.Extensions.DependencyInjection;
using Censeq.Abp.Identity.Localization;
using Censeq.Abp.Users;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Abp.Identity;

/// <summary>
/// DomainShared模块
/// </summary>
[DependsOn(
    typeof(CenseqUsersDomainSharedModule),
    typeof(AbpValidationModule),
    typeof(AbpFeaturesModule)
    )]
public class CenseqIdentityDomainSharedModule : AbpModule
{
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context"></param>
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
                ).AddVirtualJson("/Censeq/Abp/Identity/Localization");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Censeq.Abp.Identity", typeof(IdentityResource));
        });
    }
}
