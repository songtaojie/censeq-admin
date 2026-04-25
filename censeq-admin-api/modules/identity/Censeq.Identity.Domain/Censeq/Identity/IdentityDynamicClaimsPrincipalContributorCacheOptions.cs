using System;

namespace Censeq.Identity;

/// <summary>
/// 身份动态声明主体贡献者缓存选项
/// </summary>
public class IdentityDynamicClaimsPrincipalContributorCacheOptions
{
    /// <summary>
    /// TimeSpan
    /// </summary>
    public TimeSpan CacheAbsoluteExpiration { get; set; }

    public IdentityDynamicClaimsPrincipalContributorCacheOptions()
    {
        CacheAbsoluteExpiration = TimeSpan.FromHours(1);
    }
}
