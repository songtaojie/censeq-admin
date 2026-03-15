using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

public class CenseqOpenIddictClaimsPrincipalManager : ISingletonDependency
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }
    protected IOptions<CenseqOpenIddictClaimsPrincipalOptions> Options { get; }

    public CenseqOpenIddictClaimsPrincipalManager(IServiceScopeFactory serviceScopeFactory, IOptions<CenseqOpenIddictClaimsPrincipalOptions> options)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Options = options;
    }

    public virtual async Task HandleAsync(OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            foreach (var providerType in Options.Value.ClaimsPrincipalHandlers)
            {
                var provider = (ICenseqOpenIddictClaimsPrincipalHandler)scope.ServiceProvider.GetRequiredService(providerType);
                await provider.HandleAsync(new CenseqOpenIddictClaimsPrincipalHandlerContext(scope.ServiceProvider, openIddictRequest, principal));
            }
        }
    }
}
