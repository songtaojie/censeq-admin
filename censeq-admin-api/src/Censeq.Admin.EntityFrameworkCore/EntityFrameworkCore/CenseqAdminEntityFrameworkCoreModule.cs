using Censeq.AuditLogging.EntityFrameworkCore;
using Censeq.FeatureManagement.EntityFrameworkCore;
using Censeq.Identity.EntityFrameworkCore;
using Censeq.OpenIddict.EntityFrameworkCore;
using Censeq.PermissionManagement.EntityFrameworkCore;
using Censeq.SettingManagement.EntityFrameworkCore;
using Censeq.TenantManagement.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

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
    typeof(CenseqFeatureManagementEntityFrameworkCoreModule)
    )]
public class CenseqAdminEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CenseqAdminDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        var configuration = context.Services.GetConfiguration();
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<CenseqAdminDbContext>(dbContext =>
            {
                dbContext.UseDynamicSql(configuration);
            });
        });
    }
}
