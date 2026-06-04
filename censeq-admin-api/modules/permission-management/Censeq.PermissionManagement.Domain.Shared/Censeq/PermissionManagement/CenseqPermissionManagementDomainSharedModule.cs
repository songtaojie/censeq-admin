using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Censeq.PermissionManagement.Localization;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Localization.ExceptionHandling;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理领域共享模块。
/// </summary>
[DependsOn(typeof(AbpValidationModule))]
public class CenseqPermissionManagementDomainSharedModule : AbpModule
{
    /// <summary>
    /// 配置权限管理模块服务。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
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
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Censeq/PermissionManagement/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("CenseqPermissionManagement", typeof(CenseqPermissionManagementResource));
        });
    }
}
