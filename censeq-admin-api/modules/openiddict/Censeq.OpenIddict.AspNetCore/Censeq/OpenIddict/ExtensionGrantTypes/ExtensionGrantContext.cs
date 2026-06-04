using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Censeq.OpenIddict.ExtensionGrantTypes;

/// <summary>
/// 扩展授权类型上下文。
/// </summary>
public class ExtensionGrantContext
{
    /// <summary>
    /// HTTP 上下文。
    /// </summary>
    public HttpContext HttpContext { get; }

    /// <summary>
    /// 请求。
    /// </summary>
    public OpenIddictRequest Request { get; }

    /// <summary>
    /// 初始化 ExtensionGrantContext 实例。
    /// </summary>
    /// <param name="httpContext">HTTP上下文。</param>
    /// <param name="request">OpenIddict 请求。</param>
    public ExtensionGrantContext(HttpContext httpContext, OpenIddictRequest request)
    {
        HttpContext = httpContext;
        Request = request;
    }
}
