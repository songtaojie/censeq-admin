using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 权限管理 EntityFrameworkCore 模块。
/// 注册权限管理 DbContext 以及权限组、权限定义、权限授予仓储实现。
/// </summary>
[DependsOn(typeof(CenseqPermissionManagementDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public class CenseqPermissionManagementEntityFrameworkCoreModule : AbpModule
{
    /// <summary>
    /// 配置权限管理 EF Core 服务。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<PermissionManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<IPermissionManagementDbContext>();
            options.AddRepository<PermissionGroup, EfCorePermissionGroupRepository>();
            options.AddRepository<PermissionDefinitionRecord, EfCorePermissionDefinitionRecordRepository>();
            options.AddRepository<PermissionGrant, EfCorePermissionGrantRepository>();
        });
    }
}
