using System;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 标识转换器。
/// </summary>
public class CenseqOpenIddictIdentifierConverter : ITransientDependency
{
    /// <summary>
    /// FromString。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <returns>操作结果。</returns>
    public virtual Guid FromString(string identifier)
    {
        return string.IsNullOrEmpty(identifier) ? default : Guid.Parse(identifier);
    }

    /// <summary>
    /// 转换为字符串。
    /// </summary>
    /// <param name="identifier">标识。</param>
    /// <returns>操作结果。</returns>
    public virtual string ToString(Guid identifier)
    {
        return identifier.ToString("D");
    }
}
