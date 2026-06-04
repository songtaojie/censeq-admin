using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Volo.Abp;

namespace Censeq.OpenIddict.WildcardDomains;

/// <summary>
/// 验证授权方。
/// </summary>
public class CenseqValidateAuthorizedParty : CenseqOpenIddictWildcardDomainBase<CenseqValidateAuthorizedParty, OpenIddictServerHandlers.Session.ValidateAuthorizedParty, OpenIddictServerEvents.ValidateLogoutRequestContext>
{
    /// <summary>
    /// 描述符。
    /// </summary>
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.ValidateLogoutRequestContext>()
            .UseScopedHandler<CenseqValidateAuthorizedParty>()
            .SetOrder(OpenIddictServerHandlers.Session.ValidateEndpointPermissions.Descriptor.Order + 1_000)
            .SetType(OpenIddictServerHandlerType.BuiltIn)
            .Build();

    /// <summary>
    /// 初始化 CenseqValidateAuthorizedParty 实例。
    /// </summary>
    /// <param name="wildcardDomainsOptions">通配域名Domains配置项。</param>
    /// <param name="applicationManager">OpenIddict 应用程序管理器。</param>
    public CenseqValidateAuthorizedParty(
        IOptions<CenseqOpenIddictWildcardDomainOptions> wildcardDomainsOptions,
        IOpenIddictApplicationManager applicationManager)
        : base(wildcardDomainsOptions, new OpenIddictServerHandlers.Session.ValidateAuthorizedParty(applicationManager))
    {
        OriginalHandler = new OpenIddictServerHandlers.Session.ValidateAuthorizedParty(applicationManager);
    }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public async override ValueTask HandleAsync(OpenIddictServerEvents.ValidateLogoutRequestContext context)
    {
        Check.NotNull(context, nameof(context));

        if (context.PostLogoutRedirectUri != null && await CheckWildcardDomainAsync(context.PostLogoutRedirectUri))
        {
            return;
        }

        await OriginalHandler.HandleAsync(context);
    }
}
