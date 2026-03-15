using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace Censeq.OpenIddict.WildcardDomains;

public class CenseqValidateAuthorizedParty : CenseqOpenIddictWildcardDomainBase<CenseqValidateAuthorizedParty, OpenIddictServerHandlers.Session.ValidateAuthorizedParty, OpenIddictServerEvents.ValidateLogoutRequestContext>
{
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateLogoutRequestContext>()
            .UseScopedHandler<CenseqValidateAuthorizedParty>()
            .SetOrder(OpenIddictServerHandlers.Session.ValidateEndpointPermissions.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    public CenseqValidateAuthorizedParty(
        IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions,
        IOpenIddictApplicationManager applicationManager)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Session.ValidateAuthorizedParty(applicationManager))
    {
        OriginalHandler = new OpenIddictServerHandlers.Session.ValidateAuthorizedParty(applicationManager);
    }

    public async override ValueTask HandleAsync(OpenIddictServerEvents.ValidateLogoutRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (await CheckWildcardDomainAsync(context.PostLogoutRedirectUri))
        {
            return;
        }

        await OriginalHandler.HandleAsync(context);
    }
}
