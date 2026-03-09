using Microsoft.AspNetCore.Identity;
using static Censeq.Abp.Identity.AspNetCore.CenseqSecurityStampValidatorCallback;

namespace Censeq.Abp.Identity.AspNetCore;

/// <summary>
/// ��ȫ�����֤��ѡ����չ
/// </summary>
public static class SecurityStampValidatorOptionsExtensions
{
    /// <summary>
    /// ����Principal
    /// </summary>
    /// <param name="options"></param>
    /// <param name="abpRefreshingPrincipalOptions"></param>
    /// <returns></returns>
    public static SecurityStampValidatorOptions UpdatePrincipal(this SecurityStampValidatorOptions options, CenseqRefreshingPrincipalOptions abpRefreshingPrincipalOptions)
    {
        var previousOnRefreshingPrincipal = options.OnRefreshingPrincipal;
        options.OnRefreshingPrincipal = async context =>
        {
            await SecurityStampValidatorCallback.UpdatePrincipal(context, abpRefreshingPrincipalOptions);
            if (previousOnRefreshingPrincipal != null)
            {
                await previousOnRefreshingPrincipal.Invoke(context);
            }
        };
        return options;
    }
}
