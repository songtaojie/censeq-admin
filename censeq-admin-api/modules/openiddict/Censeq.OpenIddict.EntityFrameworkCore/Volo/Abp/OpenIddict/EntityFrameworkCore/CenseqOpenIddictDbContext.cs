using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;

namespace Censeq.OpenIddict.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(CenseqOpenIddictDbProperties.ConnectionStringName)]
public class CenseqOpenIddictDbContext : AbpDbContext<CenseqOpenIddictDbContext>, ICenseqOpenIddictDbContext
{
    public DbSet<OpenIddictApplication> Applications { get; set; }

    public DbSet<OpenIddictAuthorization> Authorizations { get; set; }

    public DbSet<OpenIddictScope> Scopes { get; set; }

    public DbSet<OpenIddictToken> Tokens { get; set; }

    public CenseqOpenIddictDbContext(DbContextOptions<CenseqOpenIddictDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureOpenIddict();
    }
}
