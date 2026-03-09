using Volo.Abp.Application.Services;
using Censeq.Abp.Identity.Localization;
using Volo.Abp.DependencyInjection;

namespace Censeq.Abp.Identity;
/// <summary>
/// 认证应用服务基类
/// </summary>
public abstract class IdentityAppServiceBase : ApplicationService
{
    /// <summary>
    /// 认证应用服务基类
    /// </summary>
    protected IdentityAppServiceBase(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        ObjectMapperContext = typeof(CenseqIdentityApplicationModule);
        LocalizationResource = typeof(IdentityResource);
        LazyServiceProvider = abpLazyServiceProvider;
    }
}
