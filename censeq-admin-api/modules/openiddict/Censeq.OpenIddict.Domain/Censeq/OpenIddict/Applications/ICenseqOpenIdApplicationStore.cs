using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Censeq.OpenIddict.Applications;

public interface ICenseqOpenIdApplicationStore : IOpenIddictApplicationStore<OpenIddictApplicationModel>
{
    ValueTask<string> GetClientUriAsync(OpenIddictApplicationModel application, CancellationToken cancellationToken = default);

    ValueTask<string> GetLogoUriAsync(OpenIddictApplicationModel application, CancellationToken cancellationToken = default);
}
