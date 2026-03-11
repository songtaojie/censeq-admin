using Censeq.Framework.EntityFrameworkCore;
using Censeq.SettingManagement.Entities;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.SettingManagement.EntityFrameworkCore;

[DependsOn(typeof(CenseqSettingManagementDomainModule))]
[DependsOn(typeof(CenseqEntityFrameworkCoreModule))]
public class CenseqSettingManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SettingManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<ISettingManagementDbContext>();

            options.AddRepository<Setting, EfCoreSettingRepository>();
            options.AddRepository<SettingDefinitionRecord, EfCoreSettingDefinitionRecordRepository>();
        });

        var configuration = context.Services.GetConfiguration();
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<SettingManagementDbContext>(dbContext =>
            {
                dbContext.UseDynamicSql(configuration);
            });
        });

    }
}
