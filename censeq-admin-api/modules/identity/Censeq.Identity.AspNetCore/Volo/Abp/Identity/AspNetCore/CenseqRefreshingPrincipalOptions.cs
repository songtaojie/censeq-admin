using System.Collections.Generic;
using Volo.Abp.Security.Claims;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// 刷新主体时的配置选项
/// </summary>
public class CenseqRefreshingPrincipalOptions
{
    /// <summary>
    /// 需要保留的当前主体声明类型列表
    /// </summary>
    public List<string> CurrentPrincipalKeepClaimTypes { get; set; }

    /// <summary>
    /// 初始化 <see cref="CenseqRefreshingPrincipalOptions"/> 类的新实例
    /// </summary>
    public CenseqRefreshingPrincipalOptions()
    {
        CurrentPrincipalKeepClaimTypes = new List<string>
        {
            AbpClaimTypes.SessionId
        };
    }
}
