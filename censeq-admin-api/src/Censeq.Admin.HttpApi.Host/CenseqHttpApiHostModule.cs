using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Censeq.Admin.MultiTenancy;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Censeq.Abp.Swashbuckle;
using OpenIddict.Validation.AspNetCore;
using Censeq.Abp.AspNetCore;
using Censeq.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Censeq.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Censeq.Abp.Account.Web.Consts;
using Volo.Abp.OpenIddict;
using Volo.Abp.AspNetCore.Mvc;
using Censeq.Admin.EntityFrameworkCore;

namespace Censeq.Admin;

[DependsOn(
    typeof(StarshineAdminHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(StarshineAdminApplicationModule),
    typeof(StarshineAdminEntityFrameworkCoreModule),
    typeof(StarshineAccountWebOpenIddictModule),
    typeof(StarshineAspNetCoreMvcUIBasicThemeModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(StarshineSwashbuckleModule),
    typeof(StarshineAspNetCoreModule)
)]
public class StarshineHttpApiHostModule : AbpModule
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
            options.Applications[StarshineAccountConsts.AppName].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            options.Applications[StarshineAccountConsts.AppName].Urls[StarshineAccountConsts.PasswordReset] = "account/reset-password";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<StarshineAdminDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<StarshineAdminDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<StarshineAdminApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Censeq.Admin.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<StarshineAdminApplicationModule>(
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
