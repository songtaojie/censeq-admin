using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starshine.Admin.EntityFrameworkCore;
using Starshine.Admin.MultiTenancy;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Starshine.Abp.Swashbuckle;
using Volo.Abp.Identity.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using Starshine.Abp.AspNetCore;
using Starshine.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Starshine.Abp.AspNetCore.Mvc.UI.Theme.Basic;

namespace Starshine.Admin;

[DependsOn(
    typeof(AdminHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AdminApplicationModule),
    typeof(AdminEntityFrameworkCoreModule),
    typeof(StarshineAccountWebOpenIddictModule),
    typeof(StarshineAspNetCoreMvcUIBasicThemeModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(StarshineSwashbuckleModule),
    typeof(StarshineAspNetCoreModule)
)]
public class AdminHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Admin");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });
        //动态Api-改进在pre中配置，启动更快
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
            options.Applications[AdminConsts.AppName].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());

            options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
            options.Applications[AdminConsts.AppName].Urls[AdminConsts.PasswordReset] = "account/reset-password";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<AdminDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Starshine.Admin.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<AdminDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Starshine.Admin.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<AdminApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Starshine.Admin.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<AdminApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath,
                        $"..{Path.DirectorySeparatorChar}Starshine.Admin.Application"));
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
