using Censeq.PermissionManagement.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理 HTTP API 模块。
/// </summary>
[DependsOn(
    typeof(CenseqPermissionManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
    )]
public class CenseqPermissionManagementHttpApiModule : AbpModule
{
    /// <summary>
    /// 预配置权限管理 HTTP API 服务。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqPermissionManagementHttpApiModule).Assembly);
        });
    }

    /// <summary>
    /// 配置权限管理模块服务。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<CenseqPermissionManagementResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
