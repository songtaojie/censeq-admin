using System.Threading.Tasks;

namespace Censeq.OpenIddict;

/// <summary>
/// Censeq OpenIddict 声明主体处理器接口。
/// </summary>
public interface ICenseqOpenIddictClaimsPrincipalHandler
{
    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task HandleAsync(CenseqOpenIddictClaimsPrincipalHandlerContext context);
}
