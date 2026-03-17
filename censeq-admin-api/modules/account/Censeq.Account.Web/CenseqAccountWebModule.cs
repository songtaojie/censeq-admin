using Microsoft.AspNetCore.Mvc.RazorPages;
using Censeq.Account.Localization;
using Censeq.Account.Web.Pages.Account;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Account.Web.ProfileManagement;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AutoMapper;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Censeq.Account;
using Volo.Abp.UI.Navigation.Urls;
using Censeq.Account.Web.Consts;
using Volo.Abp.Localization;

namespace Censeq.Account.Web;

[DependsOn(
    typeof(CenseqAccountApplicationContractsModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpExceptionHandlingModule)
    )]
public class CenseqAccountWebModule : AbpModule
{
    private readonly static OneTimeRunner OneTimeRunner = new OneTimeRunner();
    
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(AccountResource), typeof(CenseqAccountWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqAccountWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAccountWebModule>();
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new CenseqAccountUserMenuContributor());
        });

        ConfigureProfileManagementPage();

        context.Services.AddAutoMapperObjectMapper<CenseqAccountWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<CenseqAccountWebAutomapperProfile>(validate: true);
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AccountRemoteServiceConsts.ModuleName);
        });

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].Urls[CenseqAccountConsts.PasswordReset] = "account/reset-password";
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AccountResource>() // 获取内置资源
                .AddVirtualJson("/Localization/Resources"); // 添加你自定义的资源目录
        });
    }

    private void ConfigureProfileManagementPage()
    {
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Account/Manage");
        });

        Configure<ProfileManagementPageOptions>(options =>
        {
            options.Contributors.Add(new AccountProfileManagementPageContributor());
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ScriptBundles
                .Configure(typeof(ManageModel).FullName ?? typeof(ManageModel).Name,
                    configuration =>
                    {
                        configuration.AddFiles("/client-proxies/account-proxy.js");
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/Password/Default.js");
                        configuration.AddFiles("/Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.js");
                    });
        });

    }
    
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper
                .ApplyEntityConfigurationToUi(
                    IdentityModuleExtensionConsts.ModuleName,
                    IdentityModuleExtensionConsts.EntityNames.User,
                    editFormTypes: new[] { typeof(AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel) }
                );
        });
    }
}
