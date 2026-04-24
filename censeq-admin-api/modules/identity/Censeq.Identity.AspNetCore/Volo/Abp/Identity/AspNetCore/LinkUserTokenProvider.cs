using IdentityUser = Censeq.Identity.Entities.IdentityUser;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// 关联用户令牌提供程序
/// </summary>
public class LinkUserTokenProvider : DataProtectorTokenProvider<IdentityUser>
{
    /// <summary>
    /// 初始化 <see cref="LinkUserTokenProvider"/> 类的新实例
    /// </summary>
    /// <param name="dataProtectionProvider">数据保护提供程序</param>
    /// <param name="options">数据保护令牌提供程序选项</param>
    /// <param name="logger">日志记录器</param>
    public LinkUserTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<DataProtectionTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<IdentityUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {

    }
}
