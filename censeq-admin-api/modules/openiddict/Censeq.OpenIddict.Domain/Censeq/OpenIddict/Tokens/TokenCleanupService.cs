using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict.Tokens;

// Note: this background task is responsible of automatically removing orphaned tokens/authorizations
// (i.e tokens that are no longer valid and ad-hoc authorizations that have no valid tokens associated).
// Import: since tokens associated to ad-hoc authorizations are not removed as part of the same operation,
// the tokens MUST be deleted before removing the ad-hoc authorizations that no longer have any token.
/// <summary>
/// 令牌清理服务。
/// </summary>
public class TokenCleanupService : ITransientDependency
{
    /// <summary>
    /// 日志记录器。
    /// </summary>
    public ILogger<TokenCleanupService> Logger { get; set; }
    /// <summary>
    /// 清理配置项。
    /// </summary>
    protected TokenCleanupOptions CleanupOptions { get; }
    /// <summary>
    /// 令牌管理器。
    /// </summary>
    protected IOpenIddictTokenManager TokenManager { get; }
    /// <summary>
    /// 授权管理器。
    /// </summary>
    protected IOpenIddictAuthorizationManager AuthorizationManager { get; }

    /// <summary>
    /// 初始化 TokenCleanupService 实例。
    /// </summary>
    /// <param name="cleanupOptions">清理配置项。</param>
    /// <param name="tokenManager">令牌管理器。</param>
    /// <param name="authorizationManager">授权管理器。</param>
    public TokenCleanupService(
        IOptionsMonitor<TokenCleanupOptions> cleanupOptions,
        IOpenIddictTokenManager tokenManager,
        IOpenIddictAuthorizationManager authorizationManager)
    {
        Logger = NullLogger<TokenCleanupService>.Instance;;

        CleanupOptions = cleanupOptions.CurrentValue;
        TokenManager = tokenManager;
        AuthorizationManager = authorizationManager;
    }

    /// <summary>
    /// 异步清理过期数据。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task CleanAsync()
    {
        Logger.LogInformation("Start cleanup.");

        if (!CleanupOptions.DisableTokenPruning)
        {
            Logger.LogInformation("Start cleanup tokens.");

            var threshold = DateTimeOffset.UtcNow - CleanupOptions.MinimumTokenLifespan;
            try
            {
                await TokenManager.PruneAsync(threshold);
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        if (!CleanupOptions.DisableAuthorizationPruning)
        {
            Logger.LogInformation("Start cleanup authorizations.");

            var threshold = DateTimeOffset.UtcNow - CleanupOptions.MinimumAuthorizationLifespan;
            try
            {
                await AuthorizationManager.PruneAsync(threshold);
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }
    }
}
