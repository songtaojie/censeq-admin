using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace Censeq.OpenIddict.WildcardDomains;

public class CenseqValidateClientRedirectUri : CenseqOpenIddictWildcardDomainBase<CenseqValidateClientRedirectUri, OpenIddictServerHandlers.Authentication.ValidateClientRedirectUri, OpenIddictServerEvents.ValidateAuthorizationRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateAuthorizationRequestContext>()
            .AddFilter<OpenIddictServerHandlerFilters.RequireDegradedModeDisabled>()
            .UseScopedHandler<CenseqValidateClientRedirectUri>()
            .SetOrder(OpenIddictServerHandlers.Authentication.ValidateResponseType.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public CenseqValidateClientRedirectUri(
        IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions,
        IOpenIddictApplicationManager applicationManager)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Authentication.ValidateClientRedirectUri(applicationManager))
    {
        OriginalHandler = new OpenIddictServerHandlers.Authentication.ValidateClientRedirectUri(applicationManager);
    }

    public async override ValueTask HandleAsync(OpenIddictServerEvents.ValidateAuthorizationRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (!string.IsNullOrEmpty(context.RedirectUri) && await CheckWildcardDomainAsync(context.RedirectUri))
        {
            return;
        }

        await OriginalHandler.HandleAsync(context);
    }
}
