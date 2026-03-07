using System;

namespace Censeq.Abp.Identity;
/// <summary>
/// ���ݶ�̬������Ҫ�����߻���ѡ��
/// </summary>
public class IdentityDynamicClaimsPrincipalContributorCacheOptions
{
    /// <summary>
    /// 
    /// </summary>
    public TimeSpan CacheAbsoluteExpiration { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public IdentityDynamicClaimsPrincipalContributorCacheOptions()
    {
        CacheAbsoluteExpiration = TimeSpan.FromHours(1);
    }
}
