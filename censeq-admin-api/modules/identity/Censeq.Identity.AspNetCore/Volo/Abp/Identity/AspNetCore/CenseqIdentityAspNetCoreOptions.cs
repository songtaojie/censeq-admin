namespace Censeq.Identity.AspNetCore;

/// <summary>
/// Censeq Identity AspNetCore 配置选项
/// </summary>
public class CenseqIdentityAspNetCoreOptions
{
    /// <summary>
    /// 是否配置认证，默认值：true
    /// </summary>
    public bool ConfigureAuthentication { get; set; } = true;
}
