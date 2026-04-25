using Censeq.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Identity.EntityFrameworkCore;

[ConnectionStringName(CenseqIdentityDbProperties.ConnectionStringName)]
/// <summary>
/// ICenseq身份数据库上下文接口
/// </summary>
public interface ICenseqIdentityDbContext : IEfCoreDbContext
{
    DbSet<IdentityUser> Users { get; }

    DbSet<IdentityRole> Roles { get; }

    DbSet<IdentityClaimType> ClaimTypes { get; }

    DbSet<OrganizationUnit> OrganizationUnits { get; }

    DbSet<IdentitySecurityLog> SecurityLogs { get; }

    DbSet<IdentityLinkUser> LinkUsers { get; }

    DbSet<IdentityUserDelegation> UserDelegations { get; }

    DbSet<IdentitySession> Sessions { get; }
}
