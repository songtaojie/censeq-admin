namespace Censeq.Identity;

public class CenseqIdentityOptions
{
    /// <summary>
    /// 外部登录提供程序Dictionary
    /// </summary>
    public ExternalLoginProviderDictionary ExternalLoginProviders { get; }

    public CenseqIdentityOptions()
    {
        ExternalLoginProviders = new ExternalLoginProviderDictionary();
    }
}
