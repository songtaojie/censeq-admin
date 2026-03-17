using System.Collections.Generic;
using System.Linq;

namespace Censeq.OpenIddict.ExtensionGrantTypes;

public class CenseqOpenIddictExtensionGrantsOptions
{
    public Dictionary<string, IExtensionGrant> Grants { get; }

    public CenseqOpenIddictExtensionGrantsOptions()
    {
        Grants = new Dictionary<string, IExtensionGrant>();
    }

    public TExtensionGrantType Find<TExtensionGrantType>(string name)
        where TExtensionGrantType : IExtensionGrant
    {
        return (TExtensionGrantType)Grants.FirstOrDefault(x => x.Key == name && x.Value is TExtensionGrantType).Value;
    }
}
