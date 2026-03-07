using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Censeq.Abp.AspNetCore.Mvc.UI.Theme.Basic.Branding
{
    public class DefaultStarshineBrandingProvider : DefaultBrandingProvider, IStarshineBrandingProvider, ITransientDependency
    {
        public virtual string? AppDescription => "MyApplication";
    }
}
