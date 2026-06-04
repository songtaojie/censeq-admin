using Microsoft.AspNetCore.Http;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using Volo.Abp;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict HTTP 上下文扩展方法。
/// </summary>
public static class AbpOpenIddictHttpContextExtensions
{
    /// <summary>
    /// 获取 OpenIddict 服务端事务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>操作结果。</returns>
    public static OpenIddictServerTransaction GetOpenIddictServerTransaction(this HttpContext context)
    {
        Check.NotNull(context, nameof(context));
        return context.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction!;
    }
}
