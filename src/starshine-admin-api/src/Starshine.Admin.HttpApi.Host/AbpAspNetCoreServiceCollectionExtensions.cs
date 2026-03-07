using Microsoft.Extensions.DependencyInjection;
using System;

namespace Starshine.Admin
{
    public static class AbpAspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection ForwardIdentityAuthenticationForBearer(this IServiceCollection services, string jwtBearerScheme = "Bearer")
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.ForwardDefaultSelector = ctx =>
                {
                    string? authorization = ctx.Request.Headers.Authorization;
                    if (!string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return jwtBearerScheme;
                    }

                    return null;
                };
            });

            return services;
        }
    }

}
