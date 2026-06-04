using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Censeq.Account.Settings;
using Censeq.Account.Web.Settings;
using Censeq.Identity;
using Censeq.Identity.AspNetCore;
using Censeq.Identity.Entities;
using Volo.Abp.Caching;
using Volo.Abp.Data;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 选择企业页面模型。
/// </summary>
public class SelectEnterpriseModel : LoginModel
{
    /// <summary>
    /// 令牌。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    /// <summary>
    /// 已选择用户标识。
    /// </summary>
    [BindProperty]
    public Guid SelectedUserId { get; set; }

    /// <summary>
    /// 登录名称。
    /// </summary>
    public string? LoginName { get; set; }

    /// <summary>
    /// 配置项。
    /// </summary>
    public List<LoginTenantSelectionOption> Options { get; set; } = new();

    /// <summary>
    /// 初始化 SelectEnterpriseModel 实例。
    /// </summary>
    /// <param name="schemeProvider">认证方案提供者。</param>
    /// <param name="accountOptions">账户配置项。</param>
    /// <param name="identityDynamicClaimsPrincipalContributorCache">Identity 动态声明主体贡献器缓存。</param>
    /// <param name="webHostEnvironment">Web 主机环境。</param>
    /// <param name="identitySessionManager">Identity 会话管理器。</param>
    public SelectEnterpriseModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<CenseqAccountOptions> accountOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        IWebHostEnvironment webHostEnvironment,
        IdentitySessionManager identitySessionManager)
        : base(schemeProvider, accountOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment, identitySessionManager)
    {
    }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public override async Task<IActionResult> OnGetAsync()
    {
        var cacheItem = await GetSelectionCacheItemAsync();
        if (cacheItem == null)
        {
            Alerts.Warning("企业选择已失效，请重新输入邮箱和密码。");
            return RedirectToPage("./Login", new { returnUrl = ReturnUrl, returnUrlHash = ReturnUrlHash });
        }

        LoginName = cacheItem.LoginName;
        Options = cacheItem.Options;
        return Page();
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <param name="action">action。</param>
    /// <returns>页面处理结果。</returns>
    public override async Task<IActionResult> OnPostAsync(string action)
    {
        var cacheItem = await GetSelectionCacheItemAsync();
        if (cacheItem == null)
        {
            Alerts.Warning("企业选择已失效，请重新输入邮箱和密码。");
            return RedirectToPage("./Login", new { returnUrl = ReturnUrl, returnUrlHash = ReturnUrlHash });
        }

        LoginName = cacheItem.LoginName;
        Options = cacheItem.Options;

        var selectedOption = cacheItem.Options.FirstOrDefault(x => x.UserId == SelectedUserId);
        if (selectedOption == null)
        {
            Alerts.Warning("请选择一个登录企业。");
            return Page();
        }

        var userRepository = LazyServiceProvider.LazyGetRequiredService<IIdentityUserRepository>();
        IdentityUser user;
        using (LazyServiceProvider.LazyGetRequiredService<IDataFilter>().Disable<Volo.Abp.MultiTenancy.IMultiTenant>())
        {
            user = await userRepository.GetAsync(selectedOption.UserId);
        }

        ApplyTenantContext(user.TenantId);
        await IdentityOptions.SetAsync();
        await SignInManager.SignInAsync(user, cacheItem.RememberMe);

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.LoginSucceeded,
            UserName = user.UserName ?? cacheItem.LoginName
        });

        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
        await CreateSessionAsync(user);
        await RemoveSelectionCacheItemAsync();

        return await RedirectSafelyAsync(cacheItem.ReturnUrl ?? string.Empty, cacheItem.ReturnUrlHash);
    }

    /// <summary>
    /// 异步获取选择缓存项。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<LoginTenantSelectionCacheItem?> GetSelectionCacheItemAsync()
    {
        if (Token.IsNullOrWhiteSpace())
        {
            return null;
        }

        var cache = LazyServiceProvider.LazyGetRequiredService<IDistributedCache<LoginTenantSelectionCacheItem>>();
        return await cache.GetAsync(Token!, considerUow: true);
    }

    /// <summary>
    /// 异步移除租户选择缓存项。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task RemoveSelectionCacheItemAsync()
    {
        if (Token.IsNullOrWhiteSpace())
        {
            return;
        }

        var cache = LazyServiceProvider.LazyGetRequiredService<IDistributedCache<LoginTenantSelectionCacheItem>>();
        await cache.RemoveAsync(Token!, considerUow: true);
    }
}