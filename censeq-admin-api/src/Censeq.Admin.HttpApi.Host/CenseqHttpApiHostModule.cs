using Censeq.Admin.MultiTenancy;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Censeq.Admin.EntityFrameworkCore;
using Censeq.Framework.AspNetCore.Mvc.UI.Theme.Basic;
using Censeq.Framework.Swashbuckle;
using Censeq.Framework.AspNetCore;
using Censeq.Account.Web;
using Volo.Abp.OpenIddict;
using OpenIddict.Validation.AspNetCore;
using Censeq.Account.Web.Consts;

namespace Censeq.Admin;

[DependsOn(
    typeof(CenseqAdminHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(CenseqAdminApplicationModule),
    typeof(CenseqAdminEntityFrameworkCoreModule),
    typeof(CenseqAccountWebOpenIddictModule),
    typeof(CenseqAspNetCoreMvcUIBasicThemeModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(CenseqSwashbuckleModule),
    typeof(CenseqAspNetCoreModule)
)]
public class CenseqHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                //options.AddAudiences("Admin");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });
            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                //serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
       
        //��̬Api-�Ľ���pre�����ã���������
        //PreConfigure<AbpAspNetCoreMvcOptions>(options =>
        //{
        //    options.ConventionalControllers.Create(typeof(AdminApplicationModule).Assembly, opts =>
        //    {
        //        opts.TypePredicate = type => { return false; };
        //    });
        //});
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureVirtualFileSystem(context);
        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });

        //Configure<AbpSecurityLogOptions>(options =>
        //{
        //    options.IsEnabled = false;
        //});
    }

    private static void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications[CenseqAccountConsts.AppName].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            options.Applications[CenseqAccountConsts.AppName].Urls[CenseqAccountConsts.PasswordReset] = "account/reset-password";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<CenseqAdminDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<CenseqAdminDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<CenseqAdminApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<CenseqAdminApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Application"));
            });
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        //if (!env.IsDevelopment())
        //{
        //    app.UseErrorPage();
        //}
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        //app.UseCors();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        
        app.UseConfiguredEndpoints();
    }
}
