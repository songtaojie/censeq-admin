namespace Censeq.Abp.Identity.AspNetCore;
/// <summary>
/// 身份AspNetCore选项
/// </summary>
public class CenseqIdentityAspNetCoreOptions
{
    /// <summary>
    /// 配置身份验证
    /// 默认值: true.
    /// </summary>
    public bool ConfigureAuthentication { get; set; } = true;
}
