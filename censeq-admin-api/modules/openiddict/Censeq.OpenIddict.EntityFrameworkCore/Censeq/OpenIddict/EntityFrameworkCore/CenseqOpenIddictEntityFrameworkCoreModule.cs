using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;

namespace Censeq.OpenIddict.EntityFrameworkCore;

/// <summary>
/// OpenIddict Entity Framework Core 模块。
/// </summary>
[DependsOn(
    typeof(CenseqOpenIddictDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class CenseqOpenIddictEntityFrameworkCoreModule : AbpModule
{
    /// <summary>
    /// 配置 OpenIddict Entity Framework Core 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqOpenIddictDbContext>(options =>
        {
            options.AddDefaultRepositories<ICenseqOpenIddictDbContext>();

            options.AddRepository<OpenIddictApplication, EfCoreOpenIddictApplicationRepository>();
            options.AddRepository<OpenIddictAuthorization, EfCoreOpenIddictAuthorizationRepository>();
            options.AddRepository<OpenIddictScope, EfCoreOpenIddictScopeRepository>();
            options.AddRepository<OpenIddictToken, EfCoreOpenIddictTokenRepository>();
        });
    }
}
