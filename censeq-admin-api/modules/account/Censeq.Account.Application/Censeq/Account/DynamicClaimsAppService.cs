using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 动态声明应用服务，提供动态声明刷新能力。
/// </summary>
[Authorize]
public class DynamicClaimsAppService : IdentityAppServiceBase, IDynamicClaimsAppService
{
    /// <summary>
    /// Identity 动态声明主体贡献器缓存。
    /// </summary>
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }
    /// <summary>
    /// ABP 声明主体工厂。
    /// </summary>
    protected IAbpClaimsPrincipalFactory AbpClaimsPrincipalFactory { get; }
    /// <summary>
    /// 当前主体访问器。
    /// </summary>
    protected ICurrentPrincipalAccessor PrincipalAccessor { get; }

    /// <summary>
    /// 初始化 DynamicClaimsAppService 实例。
    /// </summary>
    /// <param name="identityDynamicClaimsPrincipalContributorCache">Identity 动态声明主体贡献器缓存。</param>
    /// <param name="abpClaimsPrincipalFactory">ABP 声明主体工厂。</param>
    /// <param name="principalAccessor">当前主体访问器。</param>
    public DynamicClaimsAppService(
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        IAbpClaimsPrincipalFactory abpClaimsPrincipalFactory,
        ICurrentPrincipalAccessor principalAccessor)
    {
        IdentityDynamicClaimsPrincipalContributorCache = identityDynamicClaimsPrincipalContributorCache;
        AbpClaimsPrincipalFactory = abpClaimsPrincipalFactory;
        PrincipalAccessor = principalAccessor;
    }

    /// <summary>
    /// 异步刷新动态声明。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task RefreshAsync()
    {
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(CurrentUser.GetId(), CurrentUser.TenantId);
        await AbpClaimsPrincipalFactory.CreateDynamicAsync(PrincipalAccessor.Principal);
    }
}
