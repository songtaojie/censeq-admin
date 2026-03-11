using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Censeq.Framework.EntityFrameworkCore;

namespace Censeq.TenantManagement.EntityFrameworkCore;

[DependsOn(typeof(CenseqTenantManagementDomainModule))]
[DependsOn(typeof(CenseqEntityFrameworkCoreModule))]
public class CenseqTenantManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<TenantManagementDbContext>(options =>
        {
            options.AddDefaultRepositories<ITenantManagementDbContext>();
        });
    }
}
