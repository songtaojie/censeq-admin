namespace Censeq.OpenIddict.Authorizations;

/// <summary>
/// OpenIddict 授权常量，集中声明常量。
/// </summary>
public class OpenIddictAuthorizationConsts
{
    /// <summary>
    /// 状态最大长度。
    /// </summary>
    public static int StatusMaxLength { get; set; } = 50;

    /// <summary>
    /// 主体最大长度。
    /// </summary>
    public static int SubjectMaxLength { get; set; } = 400;

    /// <summary>
    /// 类型最大长度。
    /// </summary>
    public static int TypeMaxLength { get; set; } = 50;
}
