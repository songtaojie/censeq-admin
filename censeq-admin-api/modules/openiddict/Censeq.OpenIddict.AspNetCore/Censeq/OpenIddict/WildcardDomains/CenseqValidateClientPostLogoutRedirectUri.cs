using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Volo.Abp;

namespace Censeq.OpenIddict.WildcardDomains;

/// <summary>
/// 验证客户端登出后重定向 URI。
/// </summary>
public class CenseqValidateClientPostLogoutRedirectUri : CenseqOpenIddictWildcardDomainBase<CenseqValidateClientPostLogoutRedirectUri, OpenIddictServerHandlers.Session.ValidateClientPostLogoutRedirectUri, OpenIddictServerEvents.ValidateLogoutRequestContext>
{
    /// <summary>
    /// 描述符。
    /// </summary>
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateLogoutRequestContext>()
            .AddFilter<OpenIddictServerHandlerFilters.RequireDegradedModeDisabled>()
            .AddFilter<OpenIddictServerHandlerFilters.RequirePostLogoutRedirectUriParameter>()
            .UseScopedHandler<CenseqValidateClientPostLogoutRedirectUri>()
            .SetOrder(OpenIddictServerHandlers.Session.ValidatePostLogoutRedirectUriParameter.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    /// <summary>
    /// 初始化 CenseqValidateClientPostLogoutRedirectUri 实例。
    /// </summary>
    /// <param name="wildcardDomainsOptions">通配域名Domains配置项。</param>
    /// <param name="applicationManager">OpenIddict 应用程序管理器。</param>
    public CenseqValidateClientPostLogoutRedirectUri(
        IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions,
        IOpenIddictApplicationManager applicationManager)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Session.ValidateClientPostLogoutRedirectUri(applicationManager))
    {
        OriginalHandler = new OpenIddictServerHandlers.Session.ValidateClientPostLogoutRedirectUri(applicationManager);
    }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public async override ValueTask HandleAsync(OpenIddictServerEvents.ValidateLogoutRequestContext context)
    {
        Check.NotNull(context, nameof(context));
        Check.NotNullOrEmpty(context.PostLogoutRedirectUri, nameof(context.PostLogoutRedirectUri));

        if (await CheckWildcardDomainAsync(context.PostLogoutRedirectUri))
        {
            return;
        }

        await OriginalHandler.HandleAsync(context);
    }
}
