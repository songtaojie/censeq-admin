using Censeq.AuditLogging.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Censeq.Admin.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqAuditLoggingEntityFrameworkCoreModule),
    typeof(CenseqAdminDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule)
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
