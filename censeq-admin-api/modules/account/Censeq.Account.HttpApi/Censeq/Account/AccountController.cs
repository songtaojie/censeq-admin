using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 账户控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = AccountRemoteServiceConsts.RemoteServiceName)]
[Area(AccountRemoteServiceConsts.ModuleName)]
[Route("api/account")]
public class AccountController : AbpControllerBase, IAccountAppService
{
    /// <summary>
    /// 账户应用服务。
    /// </summary>
    protected IAccountAppService AccountAppService { get; }

    /// <summary>
    /// 初始化 AccountController 实例。
    /// </summary>
    /// <param name="accountAppService">账户应用服务。</param>
    public AccountController(IAccountAppService accountAppService)
    {
        AccountAppService = accountAppService;
    }

    /// <summary>
    /// 异步注册账户。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>注册结果。</returns>
    [HttpPost]
    [Route("register")]
    public virtual Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        return AccountAppService.RegisterAsync(input);
    }

    /// <summary>
    /// 异步发送密码重置码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    [HttpPost]
    [Route("send-password-reset-code")]
    public virtual Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        return AccountAppService.SendPasswordResetCodeAsync(input);
    }

    /// <summary>
    /// 异步验证密码重置令牌。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>密码重置令牌是否有效。</returns>
    [HttpPost]
    [Route("verify-password-reset-token")]
    public virtual Task<bool> VerifyPasswordResetTokenAsync(VerifyPasswordResetTokenInput input)
    {
        return AccountAppService.VerifyPasswordResetTokenAsync(input);
    }

    /// <summary>
    /// 异步重置密码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    [HttpPost]
    [Route("reset-password")]
    public virtual Task ResetPasswordAsync(ResetPasswordDto input)
    {
        return AccountAppService.ResetPasswordAsync(input);
    }
}
