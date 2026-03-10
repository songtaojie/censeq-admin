using Censeq.SettingManagement.Entities;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Censeq.SettingManagement.EntityFrameworkCore;

[DependsOn(
    typeof(CenseqSettingManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
    )]
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
    }
}
