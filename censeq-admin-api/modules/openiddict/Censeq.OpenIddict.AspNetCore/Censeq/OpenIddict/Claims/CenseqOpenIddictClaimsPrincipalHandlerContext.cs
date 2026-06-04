using System;
using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 声明主体处理器上下文。
/// </summary>
public class CenseqOpenIddictClaimsPrincipalHandlerContext
{
     /// <summary>
     /// 作用域列表。
     /// </summary>
     public IServiceProvider ScopeServiceProvider { get; }

     /// <summary>
     /// OpenIddict 请求。
     /// </summary>
     public OpenIddictRequest OpenIddictRequest { get; }

     /// <summary>
     /// 主体。
     /// </summary>
     public ClaimsPrincipal Principal { get;}

     /// <summary>
     /// 初始化 CenseqOpenIddictClaimsPrincipalHandlerContext 实例。
     /// </summary>
     /// <param name="scopeServiceProvider">作用域服务提供者。</param>
     /// <param name="openIddictRequest">OpenIddict 请求。</param>
     /// <param name="principal">主体。</param>
     public CenseqOpenIddictClaimsPrincipalHandlerContext(IServiceProvider scopeServiceProvider, OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
     {
          ScopeServiceProvider = scopeServiceProvider;
          OpenIddictRequest = openIddictRequest;
          Principal = principal;
     }
}
