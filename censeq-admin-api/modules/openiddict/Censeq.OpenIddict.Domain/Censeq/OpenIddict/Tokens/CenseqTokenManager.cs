using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Censeq.OpenIddict.Tokens;

public class CenseqTokenManager : OpenIddictTokenManager<OpenIddictTokenModel>
{
    protected CenseqOpenIddictIdentifierConverter IdentifierConverter { get; }

    public CenseqTokenManager(
        [NotNull] [ItemNotNull] IOpenIddictTokenCache<OpenIddictTokenModel> cache,
        [NotNull] [ItemNotNull] ILogger<OpenIddictTokenManager<OpenIddictTokenModel>> logger,
        [NotNull] [ItemNotNull] IOptionsMonitor<OpenIddictCoreOptions> options,
        [NotNull] IOpenIddictTokenStoreResolver resolver,
        CenseqOpenIddictIdentifierConverter identifierConverter)
        : base(cache, logger, options, resolver)
    {
        IdentifierConverter = identifierConverter;
    }

    public async override ValueTask UpdateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken = default)
    {
        if (!Options.CurrentValue.DisableEntityCaching)
        {
            var entity = await Store.FindByIdAsync(IdentifierConverter.ToString(token.Id), cancellationToken);
            if (entity != null)
            {
                await Cache.RemoveAsync(entity, cancellationToken);
            }
        }

        await base.UpdateAsync(token, cancellationToken);
    }
}
