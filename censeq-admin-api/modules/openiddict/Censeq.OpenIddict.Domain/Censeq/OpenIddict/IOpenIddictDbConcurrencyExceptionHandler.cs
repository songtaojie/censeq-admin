using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 数据库并发异常处理器接口。
/// </summary>
public interface IOpenIddictDbConcurrencyExceptionHandler
{
    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="exception">异常。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task HandleAsync(AbpDbConcurrencyException exception);
}
