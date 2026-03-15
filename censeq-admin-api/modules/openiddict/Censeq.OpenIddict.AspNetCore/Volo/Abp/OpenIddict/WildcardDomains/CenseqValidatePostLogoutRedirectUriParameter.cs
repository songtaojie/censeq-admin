using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Server;

namespace Censeq.OpenIddict.WildcardDomains;

public class CenseqValidatePostLogoutRedirectUriParameter : CenseqOpenIddictWildcardDomainBase<CenseqValidatePostLogoutRedirectUriParameter, OpenIddictServerHandlers.Session.ValidatePostLogoutRedirectUriParameter, OpenIddictServerEvents.ValidateLogoutRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateLogoutRequestContext>()
            .UseSingletonHandler<CenseqValidatePostLogoutRedirectUriParameter>()
            .SetOrder(int.MinValue + 100_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public CenseqValidatePostLogoutRedirectUriParameter(IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Session.ValidatePostLogoutRedirectUriParameter())
    {
    }

    public async override ValueTask HandleAsync(OpenIddictServerEvents.ValidateLogoutRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (string.IsNullOrEmpty(context.PostLogoutRedirectUri) || await CheckWildcardDomainAsync(context.PostLogoutRedirectUri))
        {
            return;
        }

        await OriginalHandler.HandleAsync(context);
    }
}
