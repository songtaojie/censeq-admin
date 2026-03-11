using Volo.Abp.Application.Services;
using Censeq.TenantManagement.Localization;

namespace Censeq.TenantManagement;

public abstract class TenantManagementAppServiceBase : ApplicationService
{
    protected TenantManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(CenseqTenantManagementApplicationModule);
        LocalizationResource = typeof(CenseqTenantManagementResource);
    }
}
