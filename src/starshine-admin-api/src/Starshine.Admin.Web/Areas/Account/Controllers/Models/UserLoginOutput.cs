namespace Starshine.Admin.Web.Areas.Account.Controllers.Models;

public class UserLoginOutput
{
    public UserLoginOutput(UserLoginType result)
    {
        Result = result;
    }

    public UserLoginType Result { get; }

    public string Description => Result.ToString();
}
