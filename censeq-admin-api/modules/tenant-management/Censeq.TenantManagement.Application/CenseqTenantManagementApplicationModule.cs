using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Censeq.TenantManagement;

[DependsOn(typeof(CenseqTenantManagementDomainModule))]
[DependsOn(typeof(CenseqTenantManagementApplicationContractsModule))]
[DependsOn(typeof(AbpDddApplicationModule))]
public class CenseqTenantManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<CenseqTenantManagementApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<CenseqTenantManagementApplicationAutoMapperProfile>(validate: true);
        });
    }
}
