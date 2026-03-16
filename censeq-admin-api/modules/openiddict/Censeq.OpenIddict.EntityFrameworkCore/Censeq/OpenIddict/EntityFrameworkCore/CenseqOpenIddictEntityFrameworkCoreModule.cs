using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;

namespace Censeq.OpenIddict.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqOpenIddictDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class CenseqOpenIddictEntityFrameworkCoreModule : AbpModule
{
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
