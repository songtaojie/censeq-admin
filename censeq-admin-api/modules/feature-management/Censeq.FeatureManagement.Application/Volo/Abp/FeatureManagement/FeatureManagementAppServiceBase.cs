using Volo.Abp.Application.Services;
using Censeq.FeatureManagement.Localization;

namespace Censeq.FeatureManagement;

public abstract class FeatureManagementAppServiceBase : ApplicationService
{
    protected FeatureManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(CenseqFeatureManagementApplicationModule);
        LocalizationResource = typeof(CenseqFeatureManagementResource);
    }
}
