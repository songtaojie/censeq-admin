using System.Collections.Generic;
using System.Linq;

namespace Censeq.OpenIddict.ExtensionGrantTypes;

/// <summary>
/// OpenIddict 扩展授权类型配置项，用于配置相关行为。
/// </summary>
public class CenseqOpenIddictExtensionGrantsOptions
{
    /// <summary>
    /// 授权类型列表。
    /// </summary>
    public Dictionary<string, IExtensionGrant> Grants { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictExtensionGrantsOptions 实例。
    /// </summary>
    public CenseqOpenIddictExtensionGrantsOptions()
    {
        Grants = new Dictionary<string, IExtensionGrant>();
    }

    /// <summary>
    /// 查找数据。
    /// </summary>
    /// <param name="name">name。</param>
    /// <returns>匹配的数据。</returns>
    public TExtensionGrantType Find<TExtensionGrantType>(string name)
        where TExtensionGrantType : IExtensionGrant
    {
        return (TExtensionGrantType)Grants.FirstOrDefault(x => x.Key == name && x.Value is TExtensionGrantType).Value;
    }
}
