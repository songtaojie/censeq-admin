using Censeq.OpenIddict;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Account.Web;

/// <summary>
/// 账户 Web OpenIddict 模块。
/// </summary>
[DependsOn(
    typeof(CenseqAccountWebModule),
    typeof(CenseqOpenIddictAspNetCoreModule)
)]
public class CenseqAccountWebOpenIddictModule : AbpModule
{
    /// <summary>
    /// 预配置 账户 Web Open Iddict 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqAccountWebOpenIddictModule).Assembly);
        });
    }

    /// <summary>
    /// 配置 账户 Web Open Iddict 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAccountWebOpenIddictModule>();
        });
    }
}
