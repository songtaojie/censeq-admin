using System.Linq;
using Censeq.Identity;
using Censeq.TenantManagement;
using Censeq.TenantManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Censeq.Admin.Tenants;

/// <summary>
/// 平台侧租户扩展操作服务（重置管理员密码等）
/// </summary>
[Authorize(TenantManagementPermissions.Tenants.Default)]
public class AdminTenantAppService : AdminAppService
{
    private readonly IRepository<Tenant, Guid> _tenantRepository;
    private readonly IdentityUserManager _userManager;
    private readonly ICurrentTenant _currentTenant;

    public AdminTenantAppService(
        IRepository<Tenant, Guid> tenantRepository,
        IdentityUserManager userManager,
        ICurrentTenant currentTenant)
    {
        _tenantRepository = tenantRepository;
        _userManager = userManager;
        _currentTenant = currentTenant;
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
}
