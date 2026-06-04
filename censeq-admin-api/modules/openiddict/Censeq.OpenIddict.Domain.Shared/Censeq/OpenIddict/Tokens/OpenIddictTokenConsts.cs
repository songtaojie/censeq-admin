namespace Censeq.OpenIddict.Tokens;

/// <summary>
/// OpenIddict 令牌常量，集中声明常量。
/// </summary>
public class OpenIddictTokenConsts
{
    /// <summary>
    /// 引用标识最大长度。
    /// </summary>
    public static int ReferenceIdMaxLength { get; set; } = 100;

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
