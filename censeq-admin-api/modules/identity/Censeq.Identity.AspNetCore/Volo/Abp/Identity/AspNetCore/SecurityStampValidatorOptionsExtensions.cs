using Microsoft.AspNetCore.Identity;
using static Censeq.Identity.AspNetCore.CenseqSecurityStampValidatorCallback;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// SecurityStampValidatorOptions 扩展方法
/// </summary>
public static class SecurityStampValidatorOptionsExtensions
{
    /// <summary>
    /// 更新主体信息
    /// </summary>
    /// <param name="options">安全戳验证器选项</param>
    /// <param name="CenseqRefreshingPrincipalOptions">刷新主体选项</param>
    /// <returns>更新后的安全戳验证器选项</returns>
    /// <summary>
    /// 安全戳验证器选项
    /// </summary>
    public static SecurityStampValidatorOptions UpdatePrincipal(this SecurityStampValidatorOptions options, CenseqRefreshingPrincipalOptions CenseqRefreshingPrincipalOptions)
    {
        var previousOnRefreshingPrincipal = options.OnRefreshingPrincipal;
        options.OnRefreshingPrincipal = async context =>
        {
            await SecurityStampValidatorCallback.UpdatePrincipal(context, CenseqRefreshingPrincipalOptions);
            if(previousOnRefreshingPrincipal != null)
            {
                await previousOnRefreshingPrincipal.Invoke(context);
            }
        };
        return options;
    }
}
