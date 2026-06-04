namespace Censeq.Account.Web;

/// <summary>
/// 账户配置项。
/// </summary>
public class CenseqAccountOptions
{
    /// <summary>
    /// Default value: "Windows".
    /// </summary>
    public string WindowsAuthenticationSchemeName { get; set; }

    /// <summary>
    /// 初始化 CenseqAccountOptions 实例。
    /// </summary>
    public CenseqAccountOptions()
    {
        //TODO: This makes us depend on the Microsoft.AspNetCore.Server.IISIntegration package.
        WindowsAuthenticationSchemeName = "Windows"; //Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
    }
}
