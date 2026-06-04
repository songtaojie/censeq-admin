using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using Volo.Abp.Text.Formatting;

namespace Censeq.OpenIddict.WildcardDomains;

/// <summary>
/// OpenIddict 通配域名基类。
/// </summary>
public abstract class CenseqOpenIddictWildcardDomainBase<THandler, TOriginalHandler, TContext> : IOpenIddictServerHandler<TContext>
    where THandler : class
    where TOriginalHandler : class
    where TContext : OpenIddictServerEvents.BaseContext
{
    /// <summary>
    /// 日志记录器。
    /// </summary>
    public ILogger<THandler> Logger { get; set; }
    /// <summary>
    /// 原始处理器。
    /// </summary>
    protected TOriginalHandler OriginalHandler { get; set; }
    /// <summary>
    /// 通配域名领域配置项。
    /// </summary>
    protected CenseqOpenIddictWildcardDomainOptions WildcardDomainOptions { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictWildcardDomainBase 实例。
    /// </summary>
    /// <param name="wildcardDomainOptions">通配域名领域配置项。</param>
    /// <param name="originalHandler">original处理器。</param>
    protected CenseqOpenIddictWildcardDomainBase(IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainOptions, TOriginalHandler originalHandler)
    {
        WildcardDomainOptions = wildcardDomainOptions.Value;
        OriginalHandler = originalHandler;

        Logger = NullLogger<THandler>.Instance;
    }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public abstract ValueTask HandleAsync(TContext context);

    /// <summary>
    /// 异步检查通配域名。
    /// </summary>
    /// <param name="url">URL。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual Task<bool> CheckWildcardDomainAsync(string url)
    {
        Logger.LogDebug("Checking wildcard domain for url: {url}", url);

        foreach (var domainFormat in WildcardDomainOptions.WildcardDomainsFormat)
        {
            Logger.LogDebug("Checking wildcard domain format: {domainFormat}", domainFormat);
            var extractResult = FormattedStringValueExtracter.Extract(url, domainFormat, ignoreCase: true);
            if (extractResult.IsMatch)
            {
                Logger.LogDebug("Wildcard domain found for url: {url}", url);
                return Task.FromResult(true);
            }
        }

        foreach (var domainFormat in WildcardDomainOptions.WildcardDomainsFormat)
        {
            Logger.LogDebug("Checking wildcard domain format: {domainFormat}", domainFormat);
            if (domainFormat.Replace("{0}.", "").Equals(url, StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogDebug("Wildcard domain found for url: {url}", url);
                return Task.FromResult(true);
            }
        }

        Logger.LogDebug("Wildcard domain not found for url: {url}", url);
        return Task.FromResult(false);
    }
}
