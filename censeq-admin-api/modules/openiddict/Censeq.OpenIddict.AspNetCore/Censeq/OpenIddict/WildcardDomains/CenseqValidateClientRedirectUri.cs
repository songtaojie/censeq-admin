using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Volo.Abp;

namespace Censeq.OpenIddict.WildcardDomains;

/// <summary>
/// 验证客户端重定向 URI。
/// </summary>
public class CenseqValidateClientRedirectUri : CenseqOpenIddictWildcardDomainBase<CenseqValidateClientRedirectUri, OpenIddictServerHandlers.Authentication.ValidateClientRedirectUri, OpenIddictServerEvents.ValidateAuthorizationRequestContext>
{
    /// <summary>
    /// 描述符。
    /// </summary>
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateAuthorizationRequestContext>()
            .AddFilter<OpenIddictServerHandlerFilters.RequireDegradedModeDisabled>()
            .UseScopedHandler<CenseqValidateClientRedirectUri>()
            .SetOrder(OpenIddictServerHandlers.Authentication.ValidateResponseType.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    /// <summary>
    /// 初始化 CenseqValidateClientRedirectUri 实例。
    /// </summary>
    /// <param name="wildcardDomainsOptions">通配域名Domains配置项。</param>
    /// <param name="applicationManager">OpenIddict 应用程序管理器。</param>
    public CenseqValidateClientRedirectUri(
        IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions,
        IOpenIddictApplicationManager applicationManager)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Authentication.ValidateClientRedirectUri(applicationManager))
    {
        OriginalHandler = new OpenIddictServerHandlers.Authentication.ValidateClientRedirectUri(applicationManager);
    }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public async override ValueTask HandleAsync(OpenIddictServerEvents.ValidateAuthorizationRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (!string.IsNullOrEmpty(context.RedirectUri) && await CheckWildcardDomainAsync(context.RedirectUri))
        {
            return;
        }

        await OriginalHandler.HandleAsync(context);
    }
}
