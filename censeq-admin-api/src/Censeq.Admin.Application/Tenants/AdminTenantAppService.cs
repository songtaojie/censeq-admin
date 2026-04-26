using System.Linq;
using Censeq.Admin.Permissions;
using Censeq.Identity;
using Censeq.PermissionManagement;
using Censeq.TenantManagement;
using Censeq.TenantManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Censeq.Admin.Tenants;

/// <summary>
/// 平台侧租户扩展操作服务（重置管理员密码、权限范围管理等）
/// </summary>
[Authorize(TenantManagementPermissions.Tenants.Default)]
public class AdminTenantAppService : AdminAppService
{
    private readonly IRepository<Tenant, Guid> _tenantRepository;
    private readonly IdentityUserManager _userManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<TenantPermissionGrant, Guid> _tenantPermissionGrantRepository;
    private readonly IDistributedCache<TenantPermissionScopeCacheItem> _tenantScopeCache;

    public AdminTenantAppService(
        IRepository<Tenant, Guid> tenantRepository,
        IdentityUserManager userManager,
        ICurrentTenant currentTenant,
        IRepository<TenantPermissionGrant, Guid> tenantPermissionGrantRepository,
        IDistributedCache<TenantPermissionScopeCacheItem> tenantScopeCache)
    {
        _tenantRepository = tenantRepository;
        _userManager = userManager;
        _currentTenant = currentTenant;
        _tenantPermissionGrantRepository = tenantPermissionGrantRepository;
        _tenantScopeCache = tenantScopeCache;
    }

    /// <summary>
    /// 重置指定租户的 admin 用户密码。
    /// </summary>
    [Authorize(TenantManagementPermissions.Tenants.ResetAdminPassword)]
    public virtual async Task ResetAdminPasswordAsync(Guid tenantId, string newPassword)
    {
        Check.NotNullOrWhiteSpace(newPassword, nameof(newPassword));

        var tenant = await _tenantRepository.FindAsync(tenantId)
            ?? throw new EntityNotFoundException(typeof(Tenant), tenantId);

        using (_currentTenant.Change(tenant.Id, tenant.Name))
        {
            // 在租户上下文中查找 admin 用户（按用户名）
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                throw new BusinessException("Censeq.Admin:TenantAdminNotFound")
                    .WithData("TenantName", tenant.Name);
            }

            var removeResult = await _userManager.RemovePasswordAsync(adminUser);
            if (!removeResult.Succeeded)
            {
                throw new BusinessException().WithData("errors", string.Join(", ", removeResult.Errors.Select(e => e.Description)));
            }
            var addResult = await _userManager.AddPasswordAsync(adminUser, newPassword);
            if (!addResult.Succeeded)
            {
                throw new BusinessException().WithData("errors", string.Join(", ", addResult.Errors.Select(e => e.Description)));
            }
        }
    }

    /// <summary>
    /// 获取平台向指定租户开放的权限名称列表。
    /// </summary>
    [Authorize(AdminPermissions.TenantAdmin.TenantPermissions.Default)]
    public virtual async Task<List<string>> GetPermissionsAsync(Guid tenantId)
    {
        var grants = await _tenantPermissionGrantRepository.GetListAsync(x => x.TenantId == tenantId);
        return grants.Select(x => x.PermissionName).OrderBy(x => x).ToList();
    }

    /// <summary>
    /// 全量替换平台向指定租户开放的权限范围，并使缓存失效。
    /// </summary>
    [Authorize(AdminPermissions.TenantAdmin.TenantPermissions.Update)]
    public virtual async Task UpdatePermissionsAsync(Guid tenantId, UpdateTenantPermissionsDto input)
    {
        // 确认租户存在
        _ = await _tenantRepository.FindAsync(tenantId)
            ?? throw new EntityNotFoundException(typeof(Tenant), tenantId);

        var existing = await _tenantPermissionGrantRepository.GetListAsync(x => x.TenantId == tenantId);

        var toDelete = existing
            .Where(e => !input.GrantedPermissions.Contains(e.PermissionName))
            .ToList();
        var toAdd = input.GrantedPermissions
            .Where(p => !existing.Any(e => e.PermissionName == p))
            .Select(p => new TenantPermissionGrant(GuidGenerator.Create(), tenantId, p))
            .ToList();

        if (toDelete.Count > 0)
            await _tenantPermissionGrantRepository.DeleteManyAsync(toDelete, autoSave: false);
        if (toAdd.Count > 0)
            await _tenantPermissionGrantRepository.InsertManyAsync(toAdd, autoSave: true);
        else if (toDelete.Count > 0)
            await CurrentUnitOfWork!.SaveChangesAsync();

        // 使缓存失效，下次权限检查时重新从数据库加载
        await _tenantScopeCache.RemoveAsync($"TenantScope:{tenantId}");
    }
}
