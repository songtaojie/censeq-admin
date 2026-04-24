using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Censeq.Identity.Settings;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// Censeq 登录管理器
/// </summary>
public class CenseqSignInManager : SignInManager<IdentityUser>
{
    /// <summary>
    /// Censeq Identity 选项
    /// </summary>
    protected CenseqIdentityOptions AbpOptions { get; }

    /// <summary>
    /// 设置提供程序
    /// </summary>
    protected ISettingProvider SettingProvider { get; }

    private readonly IdentityUserManager _identityUserManager;

    /// <summary>
    /// 初始化 <see cref="CenseqSignInManager"/> 类的新实例
    /// </summary>
    /// <param name="userManager">用户管理器</param>
    /// <param name="contextAccessor">HTTP 上下文访问器</param>
    /// <param name="claimsFactory">用户声明主体工厂</param>
    /// <param name="optionsAccessor">Identity 选项</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="schemes">认证方案提供程序</param>
    /// <param name="confirmation">用户确认接口</param>
    /// <param name="options">Censeq Identity 选项</param>
    /// <param name="settingProvider">设置提供程序</param>
    public CenseqSignInManager(
        IdentityUserManager userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<IdentityUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<IdentityUser> confirmation,
        IOptions<CenseqIdentityOptions> options,
        ISettingProvider settingProvider) : base(
        userManager,
        contextAccessor,
        claimsFactory,
        optionsAccessor,
        logger,
        schemes,
        confirmation)
    {
        SettingProvider = settingProvider;
        AbpOptions = options.Value;
        _identityUserManager = userManager;
    }

    /// <summary>
    /// 使用用户名和密码登录
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="password">密码</param>
    /// <param name="isPersistent">是否持久化登录</param>
    /// <param name="lockoutOnFailure">失败时是否锁定</param>
    /// <returns>登录结果</returns>
    public async override Task<SignInResult> PasswordSignInAsync(
        string userName,
        string password,
        bool isPersistent,
        bool lockoutOnFailure)
    {
        foreach (var externalLoginProviderInfo in AbpOptions.ExternalLoginProviders.Values)
        {
            var externalLoginProvider = (IExternalLoginProvider)Context.RequestServices
                .GetRequiredService(externalLoginProviderInfo.Type);

            if (await externalLoginProvider.TryAuthenticateAsync(userName, password))
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    if (externalLoginProvider is IExternalLoginProviderWithPassword externalLoginProviderWithPassword)
                    {
                        user = await externalLoginProviderWithPassword.CreateUserAsync(userName, externalLoginProviderInfo.Name, password);
                    }
                    else
                    {
                        user = await externalLoginProvider.CreateUserAsync(userName, externalLoginProviderInfo.Name);
                    }
                }
                else
                {
                    if (externalLoginProvider is IExternalLoginProviderWithPassword externalLoginProviderWithPassword)
                    {
                        await externalLoginProviderWithPassword.UpdateUserAsync(user, externalLoginProviderInfo.Name, password);
                    }
                    else
                    {
                        await externalLoginProvider.UpdateUserAsync(user, externalLoginProviderInfo.Name);
                    }
                }

                return await SignInOrTwoFactorAsync(user, isPersistent);
            }
        }

        return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    /// <summary>
    /// 登录前检查
    /// </summary>
    /// <param name="user">用户</param>
    /// <returns>登录结果，如果检查通过则返回 null</returns>
    protected async override Task<SignInResult?> PreSignInCheck(IdentityUser user)
    {
        if (!user.IsActive)
        {
            Logger.LogWarning($"The user is not active therefore cannot login! (username: \"{user.UserName}\", id:\"{user.Id}\")");
            return SignInResult.NotAllowed;
        }

        if (user.ShouldChangePasswordOnNextLogin)
        {
            Logger.LogWarning($"The user should change password! (username: \"{user.UserName}\", id:\"{user.Id}\")");
            return SignInResult.NotAllowed;
        }

        if (await _identityUserManager.ShouldPeriodicallyChangePasswordAsync(user))
        {
            return SignInResult.NotAllowed;
        }

        return await base.PreSignInCheck(user);
    }

    /// <summary>
    /// 调用受保护的 SignInOrTwoFactorAsync 方法
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="isPersistent">是否持久化登录</param>
    /// <param name="loginProvider">登录提供程序</param>
    /// <param name="bypassTwoFactor">是否跳过双因素认证</param>
    /// <returns>登录结果</returns>
    public virtual async Task<SignInResult> CallSignInOrTwoFactorAsync(IdentityUser user, bool isPersistent, string? loginProvider = null, bool bypassTwoFactor = false)
    {
        return await base.SignInOrTwoFactorAsync(user, isPersistent, loginProvider, bypassTwoFactor);
    }
}
