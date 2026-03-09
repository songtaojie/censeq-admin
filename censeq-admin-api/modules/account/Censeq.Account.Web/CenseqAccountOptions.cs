namespace Censeq.Account.Web;

public class CenseqAccountOptions
{
    /// <summary>
    /// Default value: "Windows".
    /// </summary>
    public string WindowsAuthenticationSchemeName { get; set; }

    public CenseqAccountOptions()
    {
        //TODO: This makes us depend on the Microsoft.AspNetCore.Server.IISIntegration package.
        WindowsAuthenticationSchemeName = "Windows"; //Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme;
    }
}
