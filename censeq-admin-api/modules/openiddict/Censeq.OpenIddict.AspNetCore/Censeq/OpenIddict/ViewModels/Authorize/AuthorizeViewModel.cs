namespace Censeq.OpenIddict.ViewModels.Authorization;

/// <summary>
/// 授权视图模型。
/// </summary>
public class AuthorizeViewModel
{
    /// <summary>
    /// 应用程序名称。
    /// </summary>
    public string ApplicationName { get; set; }

    /// <summary>
    /// 作用域。
    /// </summary>
    public string Scope { get; set; }
}
