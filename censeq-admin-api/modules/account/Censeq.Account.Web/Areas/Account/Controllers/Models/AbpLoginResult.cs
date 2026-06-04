namespace Censeq.Account.Web.Areas.Account.Controllers.Models;

/// <summary>
/// ABP 登录结果。
/// </summary>
public class AbpLoginResult
{
    /// <summary>
    /// 初始化 AbpLoginResult 实例。
    /// </summary>
    /// <param name="result">结果。</param>
    public AbpLoginResult(LoginResultType result)
    {
        Result = result;
    }

    /// <summary>
    /// 结果。
    /// </summary>
    public LoginResultType Result { get; }

    /// <summary>
    /// 描述。
    /// </summary>
    public string Description => Result.ToString();
}
