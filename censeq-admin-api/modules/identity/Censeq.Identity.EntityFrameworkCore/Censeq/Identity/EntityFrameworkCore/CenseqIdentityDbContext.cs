using Censeq.Identity.Entities;
using Censeq.Identity.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Identity.EntityFrameworkCore;

/// <summary>
/// Base class for the Entity Framework database context used for identity.
/// </summary>
[ConnectionStringName(CenseqIdentityDbProperties.ConnectionStringName)]
public class CenseqIdentityDbContext : AbpDbContext<CenseqIdentityDbContext>, ICenseqIdentityDbContext
{
    /// <summary>
    /// 数据库Set<IdentityUser>
    /// </summary>
    public DbSet<IdentityUser> Users { get; set; }

    /// <summary>
    /// 数据库Set<IdentityRole>
    /// </summary>
    public DbSet<IdentityRole> Roles { get; set; }

    /// <summary>
    /// 数据库Set<Identity声明Type>
    /// </summary>
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }

    /// <summary>
    /// 数据库Set<OrganizationUnit>
    /// </summary>
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    /// <summary>
    /// 数据库Set<Identity安全Log>
    /// </summary>
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }

    /// <summary>
    /// 数据库Set<Identity关联User>
    /// </summary>
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    /// <summary>
    /// 数据库Set<Identity用户Delegation>
    /// </summary>
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    /// <summary>
    /// 数据库Set<IdentitySession>
    /// </summary>
    public DbSet<IdentitySession> Sessions { get; set; }

    public CenseqIdentityDbContext(DbContextOptions<CenseqIdentityDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureIdentity();
    }
}
