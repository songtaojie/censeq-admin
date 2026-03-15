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
public interface ICenseqOpenIddictDbContext : IEfCoreDbContext
{
    DbSet<OpenIddictApplication> Applications { get; }

    DbSet<OpenIddictAuthorization> Authorizations { get; }

    DbSet<OpenIddictScope> Scopes { get; }

    DbSet<OpenIddictToken> Tokens { get; }
}
