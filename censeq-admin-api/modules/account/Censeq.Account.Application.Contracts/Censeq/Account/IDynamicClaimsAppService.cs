using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Censeq.Account;

/// <summary>
/// 动态声明应用服务接口。
/// </summary>
public interface IDynamicClaimsAppService : IApplicationService
{
    /// <summary>
    /// 异步刷新动态声明。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task RefreshAsync();
}
