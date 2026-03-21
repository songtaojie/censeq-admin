using Censeq.Account;
using Censeq.FeatureManagement;
using Censeq.Identity;
using Censeq.PermissionManagement;
using Censeq.SettingManagement;
using Censeq.TenantManagement;
using Volo.Abp.Authorization;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace Censeq.Admin;

[DependsOn(
    typeof(CenseqAdminDomainSharedModule),
    typeof(CenseqAccountApplicationContractsModule),
    typeof(CenseqIdentityApplicationContractsModule),
    typeof(CenseqPermissionManagementApplicationContractsModule),
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
