namespace Censeq.OpenIddict.Applications;

/// <summary>
/// OpenIddict 应用程序常量，集中声明常量。
/// </summary>
public class OpenIddictApplicationConsts
{
    /// <summary>
    /// 应用程序类型。
    /// </summary>
    public static int ApplicationTypeMaxLength { get; set; } = 50;

    /// <summary>
    /// 客户端标识。
    /// </summary>
    public static int ClientIdMaxLength { get; set; } = 100;

    /// <summary>
    /// 同意类型。
    /// </summary>
    public static int ConsentTypeMaxLength { get; set; } = 50;

    /// <summary>
    /// 客户端类型。
    /// </summary>
    public static int ClientTypeMaxLength { get; set; } = 50;
}
