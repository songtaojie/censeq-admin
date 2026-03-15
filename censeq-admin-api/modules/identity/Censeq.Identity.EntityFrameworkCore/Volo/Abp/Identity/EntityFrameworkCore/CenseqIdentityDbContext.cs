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
    public DbSet<IdentityUser> Users { get; set; }

    public DbSet<IdentityRole> Roles { get; set; }

    public DbSet<IdentityClaimType> ClaimTypes { get; set; }

    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }

    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

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
