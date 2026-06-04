using Microsoft.AspNetCore.Authentication;
using OpenIddict.Validation.AspNetCore;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// 应用程序构建器 ABP OpenIddict 中间件扩展。
/// </summary>
public static class ApplicationBuilderAbpOpenIddictMiddlewareExtension
{
    /// <summary>
    /// 启用 ABP OpenIddict 验证。
    /// </summary>
    /// <param name="app">app。</param>
    /// <param name="schema">schema。</param>
    /// <returns>操作结果。</returns>
    public static IApplicationBuilder UseAbpOpenIddictValidation(this IApplicationBuilder app, string schema = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
    {
        return app.Use(async (ctx, next) =>
        {
            if (ctx.User.Identity?.IsAuthenticated != true)
            {
                var result = await ctx.AuthenticateAsync(schema);
                if (result.Succeeded && result.Principal != null)
                {
                    ctx.User = result.Principal;
                }
            }

            await next();
        });
    }
}
