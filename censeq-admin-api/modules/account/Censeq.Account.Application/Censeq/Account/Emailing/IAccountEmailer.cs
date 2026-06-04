using System.Threading.Tasks;
using Censeq.Identity.Entities;

namespace Censeq.Account.Emailing;

/// <summary>
/// 账户邮件发送器接口。
/// </summary>
public interface IAccountEmailer
{
    /// <summary>
    /// 异步发送密码重置链接。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="resetToken">重置令牌。</param>
    /// <param name="appName">应用程序名称。</param>
    /// <param name="returnUrl">返回地址。</param>
    /// <param name="returnUrlHash">返回地址哈希。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task SendPasswordResetLinkAsync(
        IdentityUser user,
        string resetToken,
        string appName,
        string? returnUrl = null,
        string? returnUrlHash = null
    );
}
