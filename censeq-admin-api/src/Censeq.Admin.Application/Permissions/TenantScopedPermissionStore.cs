using Censeq.Admin.Permissions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement;

/// <summary>
/// 租户范围感知的权限存储。
/// 当处于租户上下文时，额外检查 <see cref="TenantPermissionGrant"/> 表；
/// 只有平台已向该租户开放该权限，才允许正常的授权链继续生效。
/// </summary>
[ExposeServices(typeof(IPermissionStore))]
[Dependency(ReplaceServices = true)]
public class TenantScopedPermissionStore : PermissionStore
{
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<TenantPermissionGrant, Guid> _tenantPermissionGrantRepository;
    private readonly IDistributedCache<TenantPermissionScopeCacheItem> _tenantScopeCache;

    public TenantScopedPermissionStore(
        IPermissionGrantRepository permissionGrantRepository,
        IDistributedCache<PermissionGrantCacheItem> cache,
        IPermissionDefinitionManager permissionDefinitionManager,
        ICurrentTenant currentTenant,
        IRepository<TenantPermissionGrant, Guid> tenantPermissionGrantRepository,
        IDistributedCache<TenantPermissionScopeCacheItem> tenantScopeCache,
        ILogger<TenantScopedPermissionStore> logger)
        : base(permissionGrantRepository, cache, permissionDefinitionManager)
    {
        _currentTenant = currentTenant;
        _tenantPermissionGrantRepository = tenantPermissionGrantRepository;
        _tenantScopeCache = tenantScopeCache;
        Logger = logger;
    }

    public override async Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        // 宿主上下文：直接走正常流程，不受租户范围限制
        if (!_currentTenant.IsAvailable)
        {
            return await base.IsGrantedAsync(name, providerName, providerKey);
        }

        // 租户上下文：先检查平台是否已向该租户开放此权限
        if (!await IsTenantPermissionAllowedAsync(_currentTenant.Id!.Value, name))
        {
            return false;
        }

        return await base.IsGrantedAsync(name, providerName, providerKey);
    }

    public override async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
    {
        // 宿主上下文：直接走正常流程
        if (!_currentTenant.IsAvailable)
        {
            return await base.IsGrantedAsync(names, providerName, providerKey);
        }

        var result = await base.IsGrantedAsync(names, providerName, providerKey);

        // 租户上下文：对已授予的权限再做范围过滤
        var tenantId = _currentTenant.Id!.Value;
        var grantedPermissions = result.Result
            .Where(x => x.Value == PermissionGrantResult.Granted)
            .Select(x => x.Key)
            .ToList();

        foreach (var permissionName in grantedPermissions)
        {
            if (!await IsTenantPermissionAllowedAsync(tenantId, permissionName))
            {
                result.Result[permissionName] = PermissionGrantResult.Prohibited;
            }
        }

        return result;
    }

    /// <summary>
    /// 检查租户是否被平台授予了指定权限范围（带缓存）。
    /// </summary>
    private async Task<bool> IsTenantPermissionAllowedAsync(Guid tenantId, string permissionName)
    {
        var cacheKey = $"TenantScope:{tenantId}";
        var cacheItem = await _tenantScopeCache.GetOrAddAsync(
            cacheKey,
            async () =>
            {
                var grants = await _tenantPermissionGrantRepository.GetListAsync(
                    x => x.TenantId == tenantId);
                return new TenantPermissionScopeCacheItem
                {
                    AllowedPermissions = grants.Select(x => x.PermissionName).ToHashSet()
                };
            });

        return cacheItem?.AllowedPermissions.Contains(permissionName) ?? false;
    }
}
