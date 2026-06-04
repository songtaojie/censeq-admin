using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using Volo.Abp;

namespace Censeq.OpenIddict.WildcardDomains;

/// <summary>
/// 验证重定向 URI 参数。
/// </summary>
public class CenseqValidateRedirectUriParameter : CenseqOpenIddictWildcardDomainBase<CenseqValidateRedirectUriParameter, OpenIddictServerHandlers.Authentication.ValidateRedirectUriParameter, OpenIddictServerEvents.ValidateAuthorizationRequestContext>
{
    /// <summary>
    /// 描述符。
    /// </summary>
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateAuthorizationRequestContext>()
            .UseSingletonHandler<CenseqValidateRedirectUriParameter>()
            .SetOrder(OpenIddictServerHandlers.Authentication.ValidateClientIdParameter.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    /// <summary>
    /// 初始化 CenseqValidateRedirectUriParameter 实例。
    /// </summary>
    /// <param name="wildcardDomainsOptions">通配域名Domains配置项。</param>
    public CenseqValidateRedirectUriParameter(IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Authentication.ValidateRedirectUriParameter())
    {
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
