using Censeq.FeatureManagement;
using Censeq.SettingManagement;
using Censeq.TenantManagement;
using Volo.Abp.Account;
using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Identity;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;

namespace Censeq.Admin;

[DependsOn(
    typeof(CenseqAdminDomainSharedModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(CenseqSettingManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule),
    typeof(AbpAuthorizationAbstractionsModule),
    typeof(AbpJsonModule),
    typeof(CenseqTenantManagementApplicationContractsModule),
    typeof(CenseqFeatureManagementApplicationContractsModule)
)]
public class CenseqAdminApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AdminDtoExtensions.Configure();
    }
}
