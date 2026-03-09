using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.Framework.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    public class CenseqEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(dbContext =>
                {
                    dbContext.DbContextOptions.UseSnakeCaseNamingConvention();
                    // 启用详细日志记录 
                    dbContext.DbContextOptions.EnableDetailedErrors();
                    dbContext.DbContextOptions.EnableSensitiveDataLogging();

                    // 使用ABP的日志系统记录EF Core日志 
                    dbContext.DbContextOptions.UseLoggerFactory(context.Services.GetRequiredService<ILoggerFactory>());
                });
            });
        }
    }
}
