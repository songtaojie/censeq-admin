using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Censeq.OpenIddict.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.OpenIddict;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class CenseqOpenIddictDomainSharedModule : AbpModule
{
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
                .AddVirtualJson("Volo/Abp/OpenIddict/Localization/OpenIddict");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("OpenIddict", typeof(CenseqOpenIddictResource));
        });
    }
}
