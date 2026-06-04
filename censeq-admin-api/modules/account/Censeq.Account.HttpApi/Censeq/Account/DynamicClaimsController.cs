using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Account;

/// <summary>
/// 动态声明控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = AccountRemoteServiceConsts.RemoteServiceName)]
[Area(AccountRemoteServiceConsts.ModuleName)]
[ControllerName("DynamicClaims")]
[Route("/api/account/dynamic-claims")]
public class DynamicClaimsController : AbpControllerBase, IDynamicClaimsAppService
{
    /// <summary>
    /// 动态声明应用服务。
    /// </summary>
    protected IDynamicClaimsAppService DynamicClaimsAppService { get; }

    /// <summary>
    /// 初始化 DynamicClaimsController 实例。
    /// </summary>
    /// <param name="dynamicClaimsAppService">动态声明应用服务。</param>
    public DynamicClaimsController(IDynamicClaimsAppService dynamicClaimsAppService)
    {
        DynamicClaimsAppService = dynamicClaimsAppService;
    }

    /// <summary>
    /// 异步刷新动态声明。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    [HttpPost]
    [Route("refresh")]
    public virtual Task RefreshAsync()
    {
        return DynamicClaimsAppService.RefreshAsync();
    }
}
