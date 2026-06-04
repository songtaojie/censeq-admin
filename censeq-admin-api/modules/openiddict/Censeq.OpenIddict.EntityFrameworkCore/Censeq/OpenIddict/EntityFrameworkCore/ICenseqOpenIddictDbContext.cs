using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;

namespace Censeq.OpenIddict.EntityFrameworkCore;

/// <summary>
/// CenseqOpenIddict 数据库上下文。
/// </summary>
[IgnoreMultiTenancy]
[ConnectionStringName(CenseqOpenIddictDbProperties.ConnectionStringName)]
public interface ICenseqOpenIddictDbContext : IEfCoreDbContext
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    DbSet<OpenIddictApplication> Applications { get; }

    /// <summary>
    /// 授权。
    /// </summary>
    DbSet<OpenIddictAuthorization> Authorizations { get; }

    /// <summary>
    /// 作用域列表。
    /// </summary>
    DbSet<OpenIddictScope> Scopes { get; }

    /// <summary>
    /// 令牌。
    /// </summary>
    DbSet<OpenIddictToken> Tokens { get; }
}
