using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Censeq.OpenIddict.Authorizations;

/// <summary>
/// 授权管理器，封装领域管理逻辑。
/// </summary>
public class CenseqAuthorizationManager : OpenIddictAuthorizationManager<OpenIddictAuthorizationModel>
{
    /// <summary>
    /// 标识转换器。
    /// </summary>
    protected CenseqOpenIddictIdentifierConverter IdentifierConverter { get; }

    /// <summary>
    /// 初始化 CenseqAuthorizationManager 实例。
    /// </summary>
    /// <param name="cache">缓存。</param>
    /// <param name="logger">logger。</param>
    /// <param name="options">配置项。</param>
    /// <param name="resolver">resolver。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
    public CenseqAuthorizationManager(
        [NotNull] [ItemNotNull] IOpenIddictAuthorizationCache<OpenIddictAuthorizationModel> cache,
        [NotNull] [ItemNotNull] ILogger<OpenIddictAuthorizationManager<OpenIddictAuthorizationModel>> logger,
        [NotNull] [ItemNotNull] IOptionsMonitor<OpenIddictCoreOptions> options,
        [NotNull] IOpenIddictAuthorizationStoreResolver resolver,
        CenseqOpenIddictIdentifierConverter identifierConverter)
        : base(cache, logger, options, resolver)
    {
        IdentifierConverter = identifierConverter;
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的数据。</returns>
    public async override ValueTask UpdateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken = default)
    {
        if (!Options.CurrentValue.DisableEntityCaching)
        {
            var entity = await Store.FindByIdAsync(IdentifierConverter.ToString(authorization.Id), cancellationToken);
            if (entity != null)
            {
                await Cache.RemoveAsync(entity, cancellationToken);
            }
        }

        await base.UpdateAsync(authorization, cancellationToken);
    }
}
