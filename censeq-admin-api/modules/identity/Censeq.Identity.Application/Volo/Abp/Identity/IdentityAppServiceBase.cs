using Volo.Abp.Application.Services;
using Censeq.Identity.Localization;

namespace Censeq.Identity;

public abstract class IdentityAppServiceBase : ApplicationService
{
    protected IdentityAppServiceBase()
    {
        ObjectMapperContext = typeof(CenseqIdentityApplicationModule);
        LocalizationResource = typeof(IdentityResource);
    }
}
