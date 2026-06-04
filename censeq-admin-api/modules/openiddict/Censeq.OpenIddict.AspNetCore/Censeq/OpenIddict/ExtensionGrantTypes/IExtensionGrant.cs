using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Censeq.OpenIddict.ExtensionGrantTypes;

/// <summary>
/// 扩展授权类型。
/// </summary>
public interface IExtensionGrant
{
    /// <summary>
    /// 名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>异步操作结果。</returns>
    Task<IActionResult> HandleAsync(ExtensionGrantContext context);
}
