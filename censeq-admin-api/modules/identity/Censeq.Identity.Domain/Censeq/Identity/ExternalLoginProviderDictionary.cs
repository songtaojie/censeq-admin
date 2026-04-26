using System.Collections.Generic;
using JetBrains.Annotations;

namespace Censeq.Identity;

/// <summary>
/// 外部登录提供程序Dictionary
/// </summary>
public class ExternalLoginProviderDictionary : Dictionary<string, ExternalLoginProviderInfo>
{
    /// <summary>
    /// 添加或替换提供程序。
    /// </summary>
    public void Add<TProvider>([NotNull] string name)
        where TProvider : IExternalLoginProvider
    {
        this[name] = new ExternalLoginProviderInfo(name, typeof(TProvider));
    }
}
