using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Censeq.OpenIddict.Tokens;

/// <summary>
/// 令牌管理器，封装领域管理逻辑。
/// </summary>
public class CenseqTokenManager : OpenIddictTokenManager<OpenIddictTokenModel>
{
    /// <summary>
    /// 标识转换器。
    /// </summary>
    protected CenseqOpenIddictIdentifierConverter IdentifierConverter { get; }

    /// <summary>
    /// 初始化 CenseqTokenManager 实例。
    /// </summary>
    /// <param name="cache">缓存。</param>
    /// <param name="logger">logger。</param>
    /// <param name="options">配置项。</param>
    /// <param name="resolver">resolver。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
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

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="token">OpenIddict 令牌。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的数据。</returns>
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
