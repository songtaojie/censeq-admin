using Censeq.Identity.Application.Contracts.Censeq.Identity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Censeq.Identity;

[DependsOn(
    typeof(CenseqIdentityDomainModule),
    typeof(CenseqIdentityApplicationContractsModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpPermissionManagementApplicationModule)
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
