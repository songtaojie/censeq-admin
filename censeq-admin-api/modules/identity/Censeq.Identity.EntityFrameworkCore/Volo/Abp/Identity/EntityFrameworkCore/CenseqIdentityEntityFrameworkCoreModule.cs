using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Users.EntityFrameworkCore;

namespace Censeq.Identity.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqIdentityDomainModule),
    typeof(AbpUsersEntityFrameworkCoreModule))]
public class CenseqIdentityEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqIdentityDbContext>(options =>
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
