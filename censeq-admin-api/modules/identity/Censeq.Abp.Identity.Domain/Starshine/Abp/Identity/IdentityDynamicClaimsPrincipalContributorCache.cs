using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Censeq.Abp.Identity.Managers;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Censeq.Abp.Identity;

/// <summary>
/// ���ݶ�̬�������幱���߻���
/// </summary>
public class IdentityDynamicClaimsPrincipalContributorCache : ITransientDependency
{
    /// <summary>
    /// ��־��¼
    /// </summary>
    protected ILogger<IdentityDynamicClaimsPrincipalContributorCache> Logger { get; }
    /// <summary>
    /// �ֲ�ʽ����
    /// </summary>
    protected IDistributedCache<AbpDynamicClaimCacheItem> DynamicClaimCache { get; }
    /// <summary>
    /// ��ǰ�⻧
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }
    /// <summary>
    /// �û�����
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// �û��������幤��
    /// </summary>
    protected IUserClaimsPrincipalFactory<IdentityUser> UserClaimsPrincipalFactory { get; }
    /// <summary>
    /// Abp ������Ҫ������Ȩ
    /// </summary>
    protected IOptions<AbpClaimsPrincipalFactoryOptions> AbpClaimsPrincipalFactoryOptions { get; }
    /// <summary>
    /// ��������
    /// </summary>
    protected IOptions<IdentityDynamicClaimsPrincipalContributorCacheOptions> CacheOptions { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dynamicClaimCache"></param>
    /// <param name="currentTenant"></param>
    /// <param name="userManager"></param>
    /// <param name="userClaimsPrincipalFactory"></param>
    /// <param name="abpClaimsPrincipalFactoryOptions"></param>
    /// <param name="cacheOptions"></param>
    /// <param name="logger"></param>
    public IdentityDynamicClaimsPrincipalContributorCache(
        IDistributedCache<AbpDynamicClaimCacheItem> dynamicClaimCache,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
        IOptions<AbpClaimsPrincipalFactoryOptions> abpClaimsPrincipalFactoryOptions,
        IOptions<IdentityDynamicClaimsPrincipalContributorCacheOptions> cacheOptions,
        ILogger<IdentityDynamicClaimsPrincipalContributorCache> logger)
    {
        DynamicClaimCache = dynamicClaimCache;
        CurrentTenant = currentTenant;
        UserManager = userManager;
        UserClaimsPrincipalFactory = userClaimsPrincipalFactory;
        AbpClaimsPrincipalFactoryOptions = abpClaimsPrincipalFactoryOptions;
        CacheOptions = cacheOptions;

        Logger = logger;
    }

    /// <summary>
    /// ��ȡ������
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public virtual async Task<AbpDynamicClaimCacheItem?> GetAsync(Guid userId, Guid? tenantId = null)
    {
        Logger.LogDebug($"��ȡ�û��Ķ�̬��������: {userId}");

        if (AbpClaimsPrincipalFactoryOptions.Value.DynamicClaims.IsNullOrEmpty())
        {
            var emptyCacheItem = new AbpDynamicClaimCacheItem();
            await DynamicClaimCache.SetAsync(AbpDynamicClaimCacheItem.CalculateCacheKey(userId, tenantId), emptyCacheItem, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheOptions.Value.CacheAbsoluteExpiration
            });

            return emptyCacheItem;
        }

        return await DynamicClaimCache.GetOrAddAsync(AbpDynamicClaimCacheItem.CalculateCacheKey(userId, tenantId), async () =>
        {
            using (CurrentTenant.Change(tenantId))
            {
                Logger.LogDebug($"Ϊ�û�: {userId}��䶯̬��������");

                var user = await UserManager.GetByIdAsync(userId);
                var principal = await UserClaimsPrincipalFactory.CreateAsync(user);

                var dynamicClaims = new AbpDynamicClaimCacheItem();
                foreach (var claimType in AbpClaimsPrincipalFactoryOptions.Value.DynamicClaims)
                {
                    var claims = principal.Claims.Where(x => x.Type == claimType).ToList();
                    if (claims.Count != 0)
                    {
                        dynamicClaims.Claims.AddRange(claims.Select(claim => new AbpDynamicClaim(claimType, claim.Value)));
                    }
                    else
                    {
                        dynamicClaims.Claims.Add(new AbpDynamicClaim(claimType, null));
                    }
                }

                return dynamicClaims;
            }
        }, () => new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheOptions.Value.CacheAbsoluteExpiration
        });
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    public virtual async Task ClearAsync(Guid userId, Guid? tenantId = null)
    {
        Logger.LogDebug($"ɾ���û�: {userId}�Ķ�̬��������");
        await DynamicClaimCache.RemoveAsync(AbpDynamicClaimCacheItem.CalculateCacheKey(userId, tenantId));
    }
}
