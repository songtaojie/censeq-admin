using Microsoft.Extensions.DependencyInjection;
using Censeq.Abp.EntityFrameworkCore;
using Censeq.Abp.PermissionManagement.Entities;
using Volo.Abp.Modularity;

namespace Censeq.Abp.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(StarshinePermissionManagementDomainModule))]
[DependsOn(typeof(StarshineEntityFrameworkCoreModule))]
public class StarshinePermissionManagementEntityFrameworkCoreModule : AbpModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<PermissionManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<IPermissionManagementDbContext>();

            options.AddRepository<PermissionGroupDefinitionRecord, EfCorePermissionGroupDefinitionRecordRepository>();
            options.AddRepository<PermissionDefinitionRecord, EfCorePermissionDefinitionRecordRepository>();
            options.AddRepository<PermissionGrant, EfCorePermissionGrantRepository>();
        });
    }
}
