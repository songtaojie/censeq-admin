using System.Collections.Generic;

namespace Censeq.OpenIddict.WildcardDomains;

public class CenseqOpenIddictWildcardDomainOptions
{
    public bool EnableWildcardDomainSupport { get; set; }

    public HashSet<string> WildcardDomainsFormat { get; }

    public CenseqOpenIddictWildcardDomainOptions()
    {
        WildcardDomainsFormat = new HashSet<string>();
    }
}
