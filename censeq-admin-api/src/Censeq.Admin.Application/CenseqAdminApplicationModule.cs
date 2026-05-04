using Censeq.Account;
using Censeq.AuditLogging;
using Censeq.FeatureManagement;
using Censeq.Identity;
using Censeq.LocalizationManagement;
using Censeq.PermissionManagement;
using Censeq.SettingManagement;
using Censeq.TenantManagement;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Admin;

[DependsOn(
    typeof(CenseqAdminDomainModule),
    typeof(CenseqAccountApplicationModule),
    typeof(CenseqAdminApplicationContractsModule),
    typeof(CenseqIdentityApplicationModule),
    typeof(CenseqPermissionManagementApplicationModule),
    typeof(CenseqSettingManagementApplicationModule),
    typeof(CenseqTenantManagementApplicationModule),
    typeof(CenseqFeatureManagementApplicationModule),
    typeof(CenseqAuditLoggingApplicationModule),
    typeof(CenseqLocalizationManagementApplicationModule)
    )]
public class CenseqAdminApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<CenseqAdminApplicationModule>();
        });
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAdminApplicationModule>();
        });
    }
}
