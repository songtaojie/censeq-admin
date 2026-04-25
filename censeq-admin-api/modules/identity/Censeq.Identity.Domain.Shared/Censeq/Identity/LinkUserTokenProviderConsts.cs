namespace Censeq.Identity;

/// <summary>
/// 关联用户令牌提供程序常量
/// </summary>
public static class LinkUserTokenProviderConsts
{
    /// <summary>
    /// 关联用户令牌提供程序名称
    /// </summary>
    public static string LinkUserTokenProviderName { get; set; } = "AbpLinkUser";

    /// <summary>
    /// 关联用户令牌用途
    /// </summary>
    public static string LinkUserTokenPurpose { get; set; } = "AbpLinkUser";

    /// <summary>
    /// 关联用户登录令牌用途
    /// </summary>
    public static string LinkUserLoginTokenPurpose { get; set; } = "AbpLinkUserLogin";
}
