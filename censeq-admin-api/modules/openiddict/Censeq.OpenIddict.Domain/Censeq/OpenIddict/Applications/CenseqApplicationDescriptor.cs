using OpenIddict.Abstractions;

namespace Censeq.OpenIddict.Applications;

/// <summary>
/// 应用程序描述符。
/// </summary>
public class CenseqApplicationDescriptor : OpenIddictApplicationDescriptor
{
    /// <summary>
    /// URI to further information about client.
    /// </summary>
    public string ClientUri { get; set; }

    /// <summary>
    /// URI to client logo.
    /// </summary>
    public string LogoUri { get; set; }
}
