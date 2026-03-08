using Censeq.AuditLogging.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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
    typeof(StarshineAdminDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule)
    )]
public class StarshineAdminEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<StarshineAdminDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
        var configuration = context.Services.GetConfiguration();
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(dbContext =>
            {
                dbContext.DbContextOptions.UseSnakeCaseNamingConvention();
                dbContext.UseDynamicSql(configuration);
                // 启用详细日志记录 
                dbContext.DbContextOptions.EnableDetailedErrors();
                dbContext.DbContextOptions.EnableSensitiveDataLogging();

                // 使用ABP的日志系统记录EF Core日志 
                dbContext.DbContextOptions.UseLoggerFactory(context.Services.GetRequiredService<ILoggerFactory>());
            });
        });
    }
}
