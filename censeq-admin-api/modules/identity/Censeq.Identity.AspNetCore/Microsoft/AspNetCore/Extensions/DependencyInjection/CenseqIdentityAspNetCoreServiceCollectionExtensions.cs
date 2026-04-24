using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Extensions.DependencyInjection;

/// <summary>
/// Censeq Identity AspNetCore 服务集合扩展方法
/// </summary>
public static class CenseqIdentityAspNetCoreServiceCollectionExtensions
{
    /// <summary>
    /// 为 Bearer 认证转发 Identity 认证
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="jwtBearerScheme">JWT Bearer 方案名称，默认值为 "Bearer"</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ForwardIdentityAuthenticationForBearer(this IServiceCollection services, string jwtBearerScheme = "Bearer")
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.ForwardDefaultSelector = ctx =>
            {
                string? authorization = ctx.Request.Headers.Authorization;
                if (!authorization.IsNullOrWhiteSpace() && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return jwtBearerScheme;
                }

                return null;
            };
        });

        return services;
    }
}
