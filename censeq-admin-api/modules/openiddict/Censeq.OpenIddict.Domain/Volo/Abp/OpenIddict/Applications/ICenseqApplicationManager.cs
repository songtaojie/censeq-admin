using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Censeq.OpenIddict.Applications;

public interface ICenseqApplicationManager : IOpenIddictApplicationManager
{
    ValueTask<string> GetClientUriAsync(object application, CancellationToken cancellationToken = default);

    ValueTask<string> GetLogoUriAsync(object application, CancellationToken cancellationToken = default);
}
