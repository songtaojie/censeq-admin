using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Censeq.Admin.Web.Pages.Account;
using Censeq.Admin.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Admin.Web.ProfileManagement;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AutoMapper;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Censeq.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Censeq.Admin.Localization;
using Censeq.Admin.Consts;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Censeq.Abp.OpenIddict;

namespace Censeq.Admin.Web;

[DependsOn(
    typeof(AdminApplicationContractsModule),
    typeof(CenseqIdentityAspNetCoreModule),
    typeof(AbpAutoMapperModule),
    typeof(CenseqOpenIddictAspNetCoreModule),
    typeof(AbpExceptionHandlingModule)
    //typeof(AbpAspNetCoreMvcUiBasicThemeModule)
    )]
public class CenseqAdminWebModule : AbpModule
{
    private readonly static OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(AdminResource), typeof(CenseqAdminWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqAdminWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAdminWebModule>();
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AbpAccountUserMenuContributor());
        });

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new AccountModuleToolbarContributor());
        });

        ConfigureProfileManagementPage();
        context.Services.AddRazorPages();
        context.Services.AddAutoMapperObjectMapper<CenseqAdminWebModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpAccountWebAutoMapperProfile>(validate: true);
        });

        Configure<DynamicJavaScriptProxyOptions>(options =>
        {
            options.DisableModule(AccountRemoteServiceConsts.ModuleName);
        });

        Configure<AbpEndpointRouterOptions>(options =>
        {
            options.EndpointConfigureActions.Add(builder =>
            {
                builder.Endpoints.MapRazorPages();
            });
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
                .Configure(typeof(ManageModel).FullName,
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
