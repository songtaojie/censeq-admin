using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Censeq.Abp.Identity;
/// <summary>
/// 身份认证应用模块
/// </summary>
[DependsOn(
    typeof(CenseqIdentityDomainModule),
    typeof(CenseqIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule)
    )]
public class CenseqIdentityApplicationModule : AbpModule
{
    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="context"></param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }
}
