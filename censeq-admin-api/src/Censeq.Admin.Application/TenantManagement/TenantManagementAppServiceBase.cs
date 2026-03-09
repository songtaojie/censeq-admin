using Volo.Abp.DependencyInjection;
using Volo.Abp.Application.Services;
using Censeq.Admin.Localization;

namespace Censeq.Abp.TenantManagement;
/// <summary>
/// 应用服务基类
/// </summary>
public abstract class TenantManagementAppServiceBase : ApplicationService
{
    /// <summary>
    /// 应用服务基类
    /// </summary>
    protected TenantManagementAppServiceBase(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        LazyServiceProvider = abpLazyServiceProvider;
        LocalizationResource = typeof(CenseqTenantManagementResource);
    }
}
