using System;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Modularity;
using static Censeq.Identity.AspNetCore.CenseqSecurityStampValidatorCallback;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// Censeq Identity AspNetCore 模块
/// </summary>
[DependsOn(
    typeof(CenseqIdentityDomainModule)
    )]
public class CenseqIdentityAspNetCoreModule : AbpModule
{
    /// <summary>
    /// 预配置服务
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IdentityBuilder>(builder =>
        {
            builder
                .AddDefaultTokenProviders()
                .AddTokenProvider<LinkUserTokenProvider>(LinkUserTokenProviderConsts.LinkUserTokenProviderName)
                .AddSignInManager<CenseqSignInManager>()
                .AddUserValidator<CenseqIdentityUserValidator>();
        });
    }

    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //(TODO: Extract an extension method like IdentityBuilder.AddCenseqSecurityStampValidator())
        context.Services.AddScoped<CenseqSecurityStampValidator>();
        context.Services.AddScoped(typeof(SecurityStampValidator<IdentityUser>), provider => provider.GetRequiredService(typeof(CenseqSecurityStampValidator)));
        context.Services.AddScoped(typeof(ISecurityStampValidator), provider => provider.GetRequiredService(typeof(CenseqSecurityStampValidator)));

        var options = context.Services.ExecutePreConfiguredActions(new CenseqIdentityAspNetCoreOptions());

        if (options.ConfigureAuthentication)
        {
            context.Services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = IdentityConstants.ApplicationScheme;
                    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();
        }
    }

    /// <summary>
    /// 后配置服务
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddOptions<SecurityStampValidatorOptions>()
            .Configure<IServiceProvider>((securityStampValidatorOptions, serviceProvider) =>
            {
                var CenseqRefreshingPrincipalOptions = serviceProvider.GetRequiredService<IOptions<CenseqRefreshingPrincipalOptions>>().Value;
                securityStampValidatorOptions.UpdatePrincipal(CenseqRefreshingPrincipalOptions);
            });
    }
}
