using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Censeq.Framework.AspNetCore.Mvc.UI.Theme.Basic.Branding
{
    public class DefaultCenseqBrandingProvider : DefaultBrandingProvider, ICenseqBrandingProvider, ITransientDependency
    {
        public virtual string? AppDescription => "MyApplication";
    }
}
