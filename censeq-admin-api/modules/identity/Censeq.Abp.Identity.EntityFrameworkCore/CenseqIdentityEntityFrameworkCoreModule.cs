using Microsoft.Extensions.DependencyInjection;
using Censeq.Abp.Users.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.Abp.Identity.EntityFrameworkCore;

/// <summary>
/// Censeq EntityFrameworkCore模块
/// </summary>
[DependsOn(
    typeof(StarshineIdentityDomainModule),
    typeof(StarshineUsersEntityFrameworkCoreModule))]
public class StarshineIdentityEntityFrameworkCoreModule : AbpModule
{
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context"></param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<IdentityDbContext>(options =>
        {
            options.AddRepository<IdentityUser, EfCoreIdentityUserRepository>();
            options.AddRepository<IdentityRole, EfCoreIdentityRoleRepository>();
            options.AddRepository<IdentityClaimType, EfCoreIdentityClaimTypeRepository>();
            options.AddRepository<OrganizationUnit, EfCoreOrganizationUnitRepository>();
            options.AddRepository<IdentitySecurityLog, EfCoreIdentitySecurityLogRepository>();
            options.AddRepository<IdentityLinkUser, EfCoreIdentityLinkUserRepository>();
            options.AddRepository<IdentityUserDelegation, EfCoreIdentityUserDelegationRepository>();
            options.AddRepository<IdentitySession, EfCoreIdentitySessionRepository>();
        });
    }
}
