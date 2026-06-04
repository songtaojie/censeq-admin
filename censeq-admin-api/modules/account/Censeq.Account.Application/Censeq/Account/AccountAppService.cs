using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;
using Censeq.Identity;
using Censeq.Account.Emailing;
using Censeq.Account.Localization;
using Censeq.Account.Settings;
using Censeq.Identity.Entities;

namespace Censeq.Account;

/// <summary>
/// 账户应用服务，提供注册和密码重置能力。
/// </summary>
public class AccountAppService : ApplicationService, IAccountAppService
{
    /// <summary>
    /// 角色仓储。
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// 用户管理器。
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// 账户邮件发送器。
    /// </summary>
    protected IAccountEmailer AccountEmailer { get; }
    /// <summary>
    /// Identity 安全日志管理器。
    /// </summary>
    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
    /// <summary>
    /// Identity 配置项。
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions { get; }

    /// <summary>
    /// 初始化 AccountAppService 实例。
    /// </summary>
    /// <param name="userManager">用户管理器。</param>
    /// <param name="roleRepository">角色仓储。</param>
    /// <param name="accountEmailer">账户邮件发送器。</param>
    /// <param name="identitySecurityLogManager">Identity 安全日志管理器。</param>
    /// <param name="identityOptions">Identity 配置项。</param>
    public AccountAppService(
        IdentityUserManager userManager,
        IIdentityRoleRepository roleRepository,
        IAccountEmailer accountEmailer,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions)
    {
        RoleRepository = roleRepository;
        AccountEmailer = accountEmailer;
        IdentitySecurityLogManager = identitySecurityLogManager;
        UserManager = userManager;
        IdentityOptions = identityOptions;

        LocalizationResource = typeof(AccountResource);
    }

    /// <summary>
    /// 异步注册账户。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>注册结果。</returns>
    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        await CheckSelfRegistrationAsync();

        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress, CurrentTenant.Id);

        input.MapExtraPropertiesTo(user);

        (await UserManager.CreateAsync(user, input.Password)).CheckErrors();

        await UserManager.SetEmailAsync(user, input.EmailAddress);
        await UserManager.AddDefaultRolesAsync(user);

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    /// <summary>
    /// 异步发送密码重置码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        var user = await GetUserByEmailAsync(input.Email);
        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
        await AccountEmailer.SendPasswordResetLinkAsync(user, resetToken, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    /// <summary>
    /// 异步验证密码重置令牌。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>密码重置令牌是否有效。</returns>
    public virtual async Task<bool> VerifyPasswordResetTokenAsync(VerifyPasswordResetTokenInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        return await UserManager.VerifyUserTokenAsync(
            user,
            UserManager.Options.Tokens.PasswordResetTokenProvider,
            UserManager<IdentityUser>.ResetPasswordTokenPurpose,
            input.ResetToken);
    }

    /// <summary>
    /// 异步重置密码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task ResetPasswordAsync(ResetPasswordDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);
        (await UserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePassword
        });
    }

    /// <summary>
    /// 异步根据邮箱获取用户。
    /// </summary>
    /// <param name="email">邮箱。</param>
    /// <returns>用户实体。</returns>
    protected virtual async Task<IdentityUser> GetUserByEmailAsync(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new UserFriendlyException(L["Volo.Account:InvalidEmailAddress", email]);
        }

        return user;
    }

    /// <summary>
    /// 异步检查是否允许自助注册。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled))
        {
            throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
        }
    }
}
