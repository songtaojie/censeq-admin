using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Censeq.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class CenseqIdentityServiceCollectionExtensions
{
    public static IdentityBuilder AddCenseqIdentity(this IServiceCollection services)
    {
        return services.AddCenseqIdentity(setupAction: null);
    }

    public static IdentityBuilder AddCenseqIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction)
    {
        //AbpRoleManager
        services.TryAddScoped<IdentityRoleManager>();
        services.TryAddScoped(typeof(RoleManager<IdentityRole>), provider => provider.GetService(typeof(IdentityRoleManager)));

        //AbpUserManager
        services.TryAddScoped<IdentityUserManager>();
        services.TryAddScoped(typeof(UserManager<IdentityUser>), provider => provider.GetService(typeof(IdentityUserManager)));

        //AbpUserStore
        services.TryAddScoped<IdentityUserStore>();
        services.TryAddScoped(typeof(IUserStore<IdentityUser>), provider => provider.GetService(typeof(IdentityUserStore)));

        //AbpRoleStore
        services.TryAddScoped<IdentityRoleStore>();
        services.TryAddScoped(typeof(IRoleStore<IdentityRole>), provider => provider.GetService(typeof(IdentityRoleStore)));

        return services
            .AddIdentityCore<IdentityUser>(setupAction)
            .AddRoles<IdentityRole>()
            .AddClaimsPrincipalFactory<CenseqUserClaimsPrincipalFactory>();
    }
}
