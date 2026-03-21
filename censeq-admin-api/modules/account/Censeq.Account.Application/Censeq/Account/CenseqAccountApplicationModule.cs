using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Censeq.Account.Settings;

namespace Censeq.Account;

[DependsOn(
    typeof(CenseqAccountApplicationContractsModule),
    typeof(Censeq.Identity.CenseqIdentityApplicationModule),
    typeof(Volo.Abp.UI.Navigation.AbpUiNavigationModule),
    typeof(Volo.Abp.Emailing.AbpEmailingModule)
)]
public class CenseqAccountApplicationModule : AbpModule
{
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
