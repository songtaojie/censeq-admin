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
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.UI.Navigation.Urls;
using Censeq.Account.Web.Consts;
using Volo.Abp.Localization;
using Censeq.Identity.AspNetCore;
using Censeq.Identity.ObjectExtending;
using Lazy.Captcha.Core;
using Lazy.Captcha.Core.Generator;
using Microsoft.Extensions.Configuration;
using SkiaSharp;

namespace Censeq.Account.Web;

/// <summary>
/// 账户 Web 模块。
/// </summary>
[DependsOn(
    typeof(CenseqAccountApplicationContractsModule),
    typeof(CenseqIdentityAspNetCoreModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpExceptionHandlingModule)
    )]
public class CenseqAccountWebModule : AbpModule
{
    /// <summary>
    /// 一次性执行器。
    /// </summary>
    private readonly static OneTimeRunner OneTimeRunner = new OneTimeRunner();
    
    /// <summary>
    /// 预配置 账户 Web 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
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

    /// <summary>
    /// 配置 账户 Web 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
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

        ConfigureCaptcha(context);
    }

    /// <summary>
    /// Configure 验证码。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    private static void ConfigureCaptcha(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddCaptcha(configuration);
    }

    /// <summary>
    /// Configure 个人资料 管理 页面。
    /// </summary>
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
    
    /// <summary>
    /// 后置配置 账户 Web 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
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
