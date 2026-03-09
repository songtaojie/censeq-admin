using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Censeq.Abp.PermissionManagement.Localization;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// 权限管理领域模块
/// </summary>
[DependsOn(
    typeof(AbpValidationModule)
    )]
public class CenseqPermissionManagementDomainSharedModule : AbpModule
{
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context"></param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqPermissionManagementDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<CenseqPermissionManagementResource>("zh-Hans")
                .AddBaseTypes(
                    typeof(AbpValidationResource)
                ).AddVirtualJson("/Censeq/Abp/PermissionManagement/Localization/Domain");
        });
    }
}
