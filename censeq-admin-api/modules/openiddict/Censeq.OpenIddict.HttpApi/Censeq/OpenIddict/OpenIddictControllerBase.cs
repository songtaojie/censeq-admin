using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Censeq.OpenIddict.Localization;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 控制器基类。
/// </summary>
public abstract class OpenIddictControllerBase : AbpControllerBase
{
    /// <summary>
    /// 初始化 OpenIddictControllerBase 实例。
    /// </summary>
    protected OpenIddictControllerBase()
    {
        LocalizationResource = typeof(CenseqOpenIddictResource);
    }
}
