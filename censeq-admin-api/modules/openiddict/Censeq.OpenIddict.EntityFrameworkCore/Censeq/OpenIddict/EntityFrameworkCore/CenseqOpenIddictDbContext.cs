using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;
using Censeq.OpenIddict.EntityFrameworkCore.Modeling;

namespace Censeq.OpenIddict.EntityFrameworkCore;

/// <summary>
/// OpenIddict 数据库上下文。
/// </summary>
[IgnoreMultiTenancy]
[ConnectionStringName(CenseqOpenIddictDbProperties.ConnectionStringName)]
public class CenseqOpenIddictDbContext : AbpDbContext<CenseqOpenIddictDbContext>, ICenseqOpenIddictDbContext
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    public DbSet<OpenIddictApplication> Applications { get; set; }

    /// <summary>
    /// 授权。
    /// </summary>
    public DbSet<OpenIddictAuthorization> Authorizations { get; set; }

    /// <summary>
    /// 作用域列表。
    /// </summary>
    public DbSet<OpenIddictScope> Scopes { get; set; }

    /// <summary>
    /// 令牌。
    /// </summary>
    public DbSet<OpenIddictToken> Tokens { get; set; }

    /// <summary>
    /// 初始化 CenseqOpenIddictDbContext 实例。
    /// </summary>
    /// <param name="options">配置项。</param>
    public CenseqOpenIddictDbContext(DbContextOptions<CenseqOpenIddictDbContext> options)
        : base(options)
    {

    }

    /// <summary>
    /// 创建模型时执行。
    /// </summary>
    /// <param name="builder">构建器。</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureOpenIddict();
    }
}
