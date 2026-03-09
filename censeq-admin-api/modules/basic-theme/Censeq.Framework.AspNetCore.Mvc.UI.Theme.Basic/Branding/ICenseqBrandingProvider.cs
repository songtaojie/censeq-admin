using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Ui.Branding;

namespace Censeq.Framework.AspNetCore.Mvc.UI.Theme.Basic.Branding
{
    /// <summary>
    ///  Censeq Branding Provider
    /// </summary>
    public interface ICenseqBrandingProvider: IBrandingProvider
    {
        /// <summary>
        /// app描述
        /// </summary>
        string? AppDescription { get; }
    }
}
