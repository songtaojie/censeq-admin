using Microsoft.Extensions.Localization;
using Censeq.Admin.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;
using Censeq.Framework.AspNetCore.Mvc.UI.Theme.Basic.Branding;

namespace Censeq.Admin;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ICenseqBrandingProvider),typeof(IBrandingProvider))]
public class AdminBrandingProvider : DefaultCenseqBrandingProvider
{
    private IStringLocalizer<CenseqAdminResource> _localizer;

    public AdminBrandingProvider(IStringLocalizer<CenseqAdminResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];

    public override string? AppDescription => _localizer["AppDescription"];
}
