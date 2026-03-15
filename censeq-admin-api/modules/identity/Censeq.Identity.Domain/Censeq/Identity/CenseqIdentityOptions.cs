namespace Censeq.Identity;

public class CenseqIdentityOptions
{
    public ExternalLoginProviderDictionary ExternalLoginProviders { get; }

    public CenseqIdentityOptions()
    {
        ExternalLoginProviders = new ExternalLoginProviderDictionary();
    }
}
