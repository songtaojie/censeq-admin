using Starshine.Admin.Localization;
using Volo.Abp.Application.Services;

namespace Starshine.Admin.FeatureManagement;

public abstract class FeatureManagementAppServiceBase : ApplicationService
{
    protected FeatureManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(StarshineAdminApplicationModule);
        LocalizationResource = typeof(StarshineFeatureManagementResource);
    }
}
