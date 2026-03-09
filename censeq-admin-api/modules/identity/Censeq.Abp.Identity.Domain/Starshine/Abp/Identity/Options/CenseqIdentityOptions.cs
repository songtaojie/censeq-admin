namespace Censeq.Abp.Identity;

/// <summary>
/// CenseqAbp 身份选项
/// </summary>
public class CenseqIdentityOptions
{
    /// <summary>
    /// 外部登录提供者词典
    /// </summary>
    public ExternalLoginProviderDictionary ExternalLoginProviders { get; }

    /// <summary>
    /// CenseqAbp 身份选项
    /// </summary>
    public CenseqIdentityOptions()
    {
        ExternalLoginProviders = [];
    }
}
