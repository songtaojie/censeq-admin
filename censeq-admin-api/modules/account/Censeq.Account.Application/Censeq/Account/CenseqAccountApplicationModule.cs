using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Censeq.Account.Settings;

namespace Censeq.Account;

/// <summary>
/// 账户应用程序模块。
/// </summary>
[DependsOn(
    typeof(CenseqAccountApplicationContractsModule),
    typeof(Censeq.Identity.CenseqIdentityApplicationModule),
    typeof(Volo.Abp.UI.Navigation.AbpUiNavigationModule),
    typeof(Volo.Abp.Emailing.AbpEmailingModule)
)]
public class CenseqAccountApplicationModule : AbpModule
{
    /// <summary>
    /// 配置 账户 应用程序 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAccountApplicationModule>();
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<CenseqAccountApplicationModuleAutoMapperProfile>(validate: true);
        });

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].Urls[AccountUrlNames.PasswordReset] = "Account/ResetPassword";
        });
    }
}
