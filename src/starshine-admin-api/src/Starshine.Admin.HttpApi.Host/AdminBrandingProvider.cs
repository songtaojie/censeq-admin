using Microsoft.Extensions.Localization;
using Starshine.Abp.AspNetCore.Mvc.UI.Theme.Basic.Branding;
using Starshine.Admin.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Starshine.Admin;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IStarshineBrandingProvider),typeof(IBrandingProvider))]
public class AdminBrandingProvider : DefaultStarshineBrandingProvider
{
    private IStringLocalizer<StarshineAdminResource> _localizer;

    public AdminBrandingProvider(IStringLocalizer<StarshineAdminResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];

    public override string? AppDescription => _localizer["AppDescription"];
}
