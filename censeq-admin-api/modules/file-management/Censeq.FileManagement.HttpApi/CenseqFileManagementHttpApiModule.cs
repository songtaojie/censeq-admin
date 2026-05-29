using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Censeq.FileManagement;

/// <summary>
/// 文件管理 HTTP API 模块，暴露文件上传、下载及存储提供器管理端点。
/// </summary>
[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(CenseqFileManagementApplicationModule)
)]
public class CenseqFileManagementHttpApiModule : AbpModule
{
}
