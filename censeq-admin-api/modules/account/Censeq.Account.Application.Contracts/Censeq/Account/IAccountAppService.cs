using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 账户应用服务接口。
/// </summary>
public interface IAccountAppService : IApplicationService
{
    /// <summary>
    /// 异步注册账户。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>注册结果。</returns>
    Task<IdentityUserDto> RegisterAsync(RegisterDto input);

    /// <summary>
    /// 异步发送密码重置码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

    /// <summary>
    /// 异步验证密码重置令牌。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>密码重置令牌是否有效。</returns>
    Task<bool> VerifyPasswordResetTokenAsync(VerifyPasswordResetTokenInput input);

    /// <summary>
    /// 异步重置密码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task ResetPasswordAsync(ResetPasswordDto input);
}
