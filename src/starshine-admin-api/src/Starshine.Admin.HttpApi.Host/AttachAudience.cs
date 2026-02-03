using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Server.OpenIddictServerEvents;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Starshine.Admin
{
    public class AttachAudience : IOpenIddictServerHandler<ProcessSignInContext>
    {
        public static OpenIddictServerHandlerDescriptor Descriptor { get; }
            = OpenIddictServerHandlerDescriptor.CreateBuilder<ProcessSignInContext>()
                .UseSingletonHandler<AttachAudience>()
                .SetOrder(OpenIddictServerHandlers.Discovery.AttachScopes.Descriptor.Order + 1)
                .Build();


        public ValueTask HandleAsync(ProcessSignInContext context)
        {
            var identity = (ClaimsIdentity)context.Principal.Identity;

            // 添加 aud 声明
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Audience, "your-audience-here"));

            return default;
        }
    }

}
