using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Account.Web;

[DependsOn(
    typeof(CenseqAccountWebModule),
    typeof(AbpOpenIddictAspNetCoreModule)
)]
public class CenseqAccountWebOpenIddictModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqAccountWebOpenIddictModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAccountWebOpenIddictModule>();
        });
    }
}
