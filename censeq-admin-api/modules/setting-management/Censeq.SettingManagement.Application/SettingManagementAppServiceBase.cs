using Censeq.SettingManagement.Localization;
using Volo.Abp.Application.Services;

namespace Censeq.SettingManagement;

public abstract class SettingManagementAppServiceBase : ApplicationService
{
    protected SettingManagementAppServiceBase()
    {
        ObjectMapperContext = typeof(CenseqSettingManagementApplicationModule);
        LocalizationResource = typeof(CenseqSettingManagementResource);
    }
}
