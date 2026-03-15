using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using Volo.Abp.Text.Formatting;

namespace Censeq.OpenIddict.WildcardDomains;

public abstract class CenseqOpenIddictWildcardDomainBase<THandler, TOriginalHandler, TContext> : IOpenIddictServerHandler<TContext>
    where THandler : class
    where TOriginalHandler : class
    where TContext : OpenIddictServerEvents.BaseContext
{
    public ILogger<THandler> Logger { get; set; }
    protected TOriginalHandler OriginalHandler { get; set; }
    protected CenseqOpenIddictWildcardDomainOptions WildcardDomainOptions { get; }

    protected CenseqOpenIddictWildcardDomainBase(IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainOptions, TOriginalHandler originalHandler)
    {
        WildcardDomainOptions = wildcardDomainOptions.Value;
        OriginalHandler = originalHandler;

        Logger = NullLogger<THandler>.Instance;
    }

    public abstract ValueTask HandleAsync(TContext context);

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
