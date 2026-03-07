using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Censeq.Abp.OpenIddict.ExtensionGrantTypes;

public interface IExtensionGrant
{
    string Name { get; }

    Task<IActionResult> HandleAsync(ExtensionGrantContext context);
}
