using Volo.Abp.Application.Services;
using Censeq.Identity.Localization;

namespace Censeq.Identity;

/// <summary>
/// 身份应用服务基类
/// </summary>
public abstract class IdentityAppServiceBase : ApplicationService
{
    protected IdentityAppServiceBase()
    {
        ObjectMapperContext = typeof(CenseqIdentityApplicationModule);
        LocalizationResource = typeof(IdentityResource);
    }
}
