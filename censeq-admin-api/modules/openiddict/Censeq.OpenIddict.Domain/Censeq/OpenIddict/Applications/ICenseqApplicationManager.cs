using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace Censeq.OpenIddict.Applications;

/// <summary>
/// Censeq应用程序管理器接口。
/// </summary>
public interface ICenseqApplicationManager : IOpenIddictApplicationManager
{
    /// <summary>
    /// 获取客户端 URI。
    /// </summary>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    ValueTask<string> GetClientUriAsync(object application, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取 Logo URI。
    /// </summary>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    ValueTask<string> GetLogoUriAsync(object application, CancellationToken cancellationToken = default);
}
