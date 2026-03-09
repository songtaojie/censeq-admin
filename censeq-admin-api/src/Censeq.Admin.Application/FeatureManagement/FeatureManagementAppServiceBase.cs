using Censeq.Admin.Localization;
using Volo.Abp.Application.Services;

namespace Censeq.Admin.FeatureManagement;

public abstract class FeatureManagementAppServiceBase : ApplicationService
{
    protected FeatureManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(CenseqAdminApplicationModule);
        LocalizationResource = typeof(CenseqFeatureManagementResource);
    }
}
