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

public class SelectEnterpriseModel : LoginModel
{
    [BindProperty(SupportsGet = true)]
    public string? Token { get; set; }

    [BindProperty]
    public Guid SelectedUserId { get; set; }

    public string? LoginName { get; set; }

    public List<LoginTenantSelectionOption> Options { get; set; } = new();

    public SelectEnterpriseModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<CenseqAccountOptions> accountOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        IWebHostEnvironment webHostEnvironment,
        IdentitySessionManager identitySessionManager)
        : base(schemeProvider, accountOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment, identitySessionManager)
    {
    }

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

    protected virtual async Task<LoginTenantSelectionCacheItem?> GetSelectionCacheItemAsync()
    {
        if (Token.IsNullOrWhiteSpace())
        {
            return null;
        }

        var cache = LazyServiceProvider.LazyGetRequiredService<IDistributedCache<LoginTenantSelectionCacheItem>>();
        return await cache.GetAsync(Token!, considerUow: true);
    }

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