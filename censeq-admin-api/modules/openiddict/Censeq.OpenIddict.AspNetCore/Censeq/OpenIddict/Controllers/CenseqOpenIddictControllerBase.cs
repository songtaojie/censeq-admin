using System.Collections.Immutable;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Volo.Abp.AspNetCore.Mvc;
using Censeq.OpenIddict.Localization;
using Volo.Abp.Security.Claims;
using Censeq.Identity;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;

namespace Censeq.OpenIddict.Controllers;

/// <summary>
/// OpenIddict 控制器基类。
/// </summary>
public abstract class CenseqOpenIddictControllerBase : AbpController
{
    /// <summary>
    /// SignIn 管理器。
    /// </summary>
    protected SignInManager<IdentityUser> SignInManager => LazyServiceProvider.LazyGetRequiredService<SignInManager<IdentityUser>>();
    /// <summary>
    /// 用户管理器。
    /// </summary>
    protected IdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();
    /// <summary>
    /// OpenIddict 应用程序管理器。
    /// </summary>
    protected IOpenIddictApplicationManager ApplicationManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictApplicationManager>();
    /// <summary>
    /// 授权管理器。
    /// </summary>
    protected IOpenIddictAuthorizationManager AuthorizationManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictAuthorizationManager>();
    /// <summary>
    /// OpenIddict 作用域管理器。
    /// </summary>
    protected IOpenIddictScopeManager ScopeManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictScopeManager>();
    /// <summary>
    /// 令牌管理器。
    /// </summary>
    protected IOpenIddictTokenManager TokenManager => LazyServiceProvider.LazyGetRequiredService<IOpenIddictTokenManager>();
    /// <summary>
    /// OpenIddict 声明主体管理器。
    /// </summary>
    protected CenseqOpenIddictClaimsPrincipalManager OpenIddictClaimsPrincipalManager => LazyServiceProvider.LazyGetRequiredService<CenseqOpenIddictClaimsPrincipalManager>();
    /// <summary>
    /// 声明主体工厂。
    /// </summary>
    protected IAbpClaimsPrincipalFactory AbpClaimsPrincipalFactory => LazyServiceProvider.LazyGetRequiredService<IAbpClaimsPrincipalFactory>();

    /// <summary>
    /// 初始化 CenseqOpenIddictControllerBase 实例。
    /// </summary>
    protected CenseqOpenIddictControllerBase()
    {
        LocalizationResource = typeof(CenseqOpenIddictResource);
    }

    /// <summary>
    /// 获取OpenIddictServer请求。
    /// </summary>
    /// <param name="httpContext">HTTP上下文。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual Task<OpenIddictRequest> GetOpenIddictServerRequestAsync(HttpContext httpContext)
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException(L["TheOpenIDConnectRequestCannotBeRetrieved"]);

        return Task.FromResult(request);
    }

    /// <summary>
    /// 获取资源。
    /// </summary>
    /// <param name="scopes">作用域。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in ScopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }
        return resources;
    }

    /// <summary>
    /// 异步检查表单值。
    /// </summary>
    /// <param name="name">name。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> HasFormValueAsync(string name)
    {
        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            if (!string.IsNullOrEmpty(form[name]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 异步执行登录前检查。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> PreSignInCheckAsync(IdentityUser user)
    {
        if (!user.IsActive)
        {
            return false;
        }

        if (!await SignInManager.CanSignInAsync(user))
        {
            return false;
        }

        if (await UserManager.IsLockedOutAsync(user))
        {
            return false;
        }

        return true;
    }
}
