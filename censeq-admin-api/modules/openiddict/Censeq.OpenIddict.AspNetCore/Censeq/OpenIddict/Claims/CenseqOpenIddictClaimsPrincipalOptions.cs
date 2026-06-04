using Volo.Abp.Collections;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 声明主体配置项，用于配置相关行为。
/// </summary>
public class CenseqOpenIddictClaimsPrincipalOptions
{
    /// <summary>
    /// 声明主体处理器列表。
    /// </summary>
    public ITypeList<ICenseqOpenIddictClaimsPrincipalHandler> ClaimsPrincipalHandlers { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictClaimsPrincipalOptions 实例。
    /// </summary>
    public CenseqOpenIddictClaimsPrincipalOptions()
    {
        ClaimsPrincipalHandlers = new TypeList<ICenseqOpenIddictClaimsPrincipalHandler>();
    }
}
