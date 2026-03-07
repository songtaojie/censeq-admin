using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Security.Claims;

namespace Censeq.Abp.Identity;

/// <summary>
/// ���ݶ�̬�������幱����
/// </summary>
public class IdentityDynamicClaimsPrincipalContributor : AbpDynamicClaimsPrincipalContributorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async override Task ContributeAsync(AbpClaimsPrincipalContributorContext context)
    {
        var identity = context.ClaimsPrincipal.Identities.FirstOrDefault();
        if (identity == null) return;
        var userId = identity.FindUserId();
        if (userId == null) return;

        var dynamicClaimsCache = context.GetRequiredService<IdentityDynamicClaimsPrincipalContributorCache>();
        AbpDynamicClaimCacheItem? dynamicClaims;
        try
        {
            dynamicClaims = await dynamicClaimsCache.GetAsync(userId.Value, identity.FindTenantId());
        }
        catch (EntityNotFoundException e)
        {
            // ����Ҳ����û������ǽ�ǿ�����������
            context.ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            var logger = context.GetRequiredService<ILogger<IdentityDynamicClaimsPrincipalContributor>>();
            logger.LogWarning(e, $"δ�ҵ��û�: {userId.Value}");
            return;
        }

        if (dynamicClaims == null || dynamicClaims.Claims.IsNullOrEmpty())
        {
            return;
        }

        await AddDynamicClaimsAsync(context, identity, dynamicClaims.Claims);
    }
}
