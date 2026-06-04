using System.Collections.Generic;

namespace Censeq.OpenIddict.WildcardDomains;

/// <summary>
/// OpenIddict 通配域名配置项，用于配置相关行为。
/// </summary>
public class CenseqOpenIddictWildcardDomainOptions
{
    /// <summary>
    /// 启用通配域名支持。
    /// </summary>
    public bool EnableWildcardDomainSupport { get; set; }

    /// <summary>
    /// 通配域名格式。
    /// </summary>
    public HashSet<string> WildcardDomainsFormat { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictWildcardDomainOptions 实例。
    /// </summary>
    public CenseqOpenIddictWildcardDomainOptions()
    {
        WildcardDomainsFormat = new HashSet<string>();
    }
}
