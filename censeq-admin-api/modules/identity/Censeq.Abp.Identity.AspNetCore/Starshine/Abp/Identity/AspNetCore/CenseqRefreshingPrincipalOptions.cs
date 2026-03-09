using System.Collections.Generic;
using Volo.Abp.Security.Claims;

namespace Censeq.Abp.Identity.AspNetCore;
/// <summary>
/// ˢ������ѡ��
/// </summary>
public class CenseqRefreshingPrincipalOptions
{
    /// <summary>
    /// ��ǰ���屣����������
    /// </summary>
    public List<string> CurrentPrincipalKeepClaimTypes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public CenseqRefreshingPrincipalOptions()
    {
        CurrentPrincipalKeepClaimTypes =
        [
            AbpClaimTypes.SessionId
        ];
    }
}
