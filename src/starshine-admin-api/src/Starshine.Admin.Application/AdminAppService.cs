using System;
using System.Collections.Generic;
using System.Text;
using Starshine.Admin.Localization;
using Volo.Abp.Application.Services;

namespace Starshine.Admin;

/* Inherit your application services from this class.
 */
public abstract class AdminAppService : ApplicationService
{
    protected AdminAppService()
    {
        LocalizationResource = typeof(StarshineAdminResource);
    }
}
