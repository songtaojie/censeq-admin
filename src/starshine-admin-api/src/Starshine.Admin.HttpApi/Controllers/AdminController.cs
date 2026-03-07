using Starshine.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Starshine.Admin.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AdminController : AbpControllerBase
{
    protected AdminController()
    {
        LocalizationResource = typeof(StarshineAdminResource);
    }
}
