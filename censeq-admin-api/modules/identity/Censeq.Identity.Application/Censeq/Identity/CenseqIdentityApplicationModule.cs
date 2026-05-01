using Censeq.PermissionManagement;
using Censeq.TenantManagement;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Censeq.Identity;

[DependsOn(
    typeof(CenseqIdentityDomainModule),
    typeof(CenseqIdentityApplicationContractsModule),
typeof(AbpAutoMapperModule),
    typeof(CenseqTenantManagementDomainModule),
    typeof(CenseqPermissionManagementApplicationModule)
    )]
public class CenseqIdentityApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<CenseqIdentityApplicationModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<CenseqIdentityApplicationModuleAutoMapperProfile>(validate: true);
        });
    }
}
