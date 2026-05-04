using Censeq.AuditLogging.EntityFrameworkCore;
using Censeq.FeatureManagement.EntityFrameworkCore;
using Censeq.Identity.EntityFrameworkCore;
using Censeq.LocalizationManagement.EntityFrameworkCore;
using Censeq.OpenIddict.EntityFrameworkCore;
using Censeq.PermissionManagement.EntityFrameworkCore;
using Censeq.SettingManagement.EntityFrameworkCore;
using Censeq.TenantManagement.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Censeq.Admin.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqAuditLoggingEntityFrameworkCoreModule),
    typeof(CenseqAdminDomainModule),
    typeof(CenseqIdentityEntityFrameworkCoreModule),
    typeof(CenseqOpenIddictEntityFrameworkCoreModule),
    typeof(CenseqPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(CenseqSettingManagementEntityFrameworkCoreModule),
    typeof(CenseqTenantManagementEntityFrameworkCoreModule),
    typeof(CenseqFeatureManagementEntityFrameworkCoreModule),
    typeof(CenseqLocalizationManagementEntityFrameworkCoreModule)
    )]
public class CenseqAdminEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqAdminDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
            options.ReplaceDbContext<IAuditLoggingDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<ICenseqIdentityDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<ICenseqOpenIddictDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<IFeatureManagementDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<IPermissionManagementDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<ISettingManagementDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<ITenantManagementDbContext>(MultiTenancySides.Both);
            options.ReplaceDbContext<ILocalizationManagementDbContext>(MultiTenancySides.Both);
        });

        var configuration = context.Services.GetConfiguration();
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<CenseqAdminDbContext>(dbContext =>
            {
                dbContext.UseDynamicSql(configuration);
                // 与 AdminDesignTimeDbContextFactory 一致：UseNpgsql 等会重建 options，需再应用 snake_case，否则 SQL 为 "Id" 而库列为 id。
                dbContext.DbContextOptions.UseSnakeCaseNamingConvention();
            });
        });
    }
}
