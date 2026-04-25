using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.Security.Claims;
using Volo.Abp.Threading;
using Censeq.Identity.Localization;

namespace Censeq.Identity;

/// <summary>
/// 身份角色管理器
/// </summary>
public class IdentityRoleManager : RoleManager<IdentityRole>, IDomainService
{
    /// <summary>
    /// Cancellation令牌
    /// </summary>
    protected override CancellationToken CancellationToken => CancellationTokenProvider.Token;

    /// <summary>
    /// IStringLocalizer<IdentityResource>
    /// </summary>
    protected IStringLocalizer<IdentityResource> Localizer { get; }
    /// <summary>
    /// ICancellation令牌提供程序
    /// </summary>
    protected ICancellationTokenProvider CancellationTokenProvider { get; }
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository UserRepository { get; }
    /// <summary>
    /// I组织单元仓储
    /// </summary>
    protected IOrganizationUnitRepository OrganizationUnitRepository { get; }
    /// <summary>
    /// 组织单元管理器
    /// </summary>
    protected OrganizationUnitManager OrganizationUnitManager { get; }
    /// <summary>
    /// IDistributedCache<Abp动态声明缓存Item>
    /// </summary>
    protected IDistributedCache<AbpDynamicClaimCacheItem> DynamicClaimCache { get; }

    public IdentityRoleManager(
        IdentityRoleStore store,
        IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<IdentityRoleManager> logger,
        IStringLocalizer<IdentityResource> localizer,
        ICancellationTokenProvider cancellationTokenProvider,
        IIdentityUserRepository userRepository,
        IOrganizationUnitRepository organizationUnitRepository,
        OrganizationUnitManager organizationUnitManager,
        IDistributedCache<AbpDynamicClaimCacheItem> dynamicClaimCache)
        : base(
              store,
              roleValidators,
              keyNormalizer,
              errors,
              logger)
    {
        Localizer = localizer;
        CancellationTokenProvider = cancellationTokenProvider;
        UserRepository = userRepository;
        OrganizationUnitRepository = organizationUnitRepository;
        OrganizationUnitManager = organizationUnitManager;
        DynamicClaimCache = dynamicClaimCache;
    }

    /// <summary>
    /// Task<IdentityRole>
    /// </summary>
    public virtual async Task<IdentityRole> GetByIdAsync(Guid id)
    {
        var role = await Store.FindByIdAsync(id.ToString(), CancellationToken);
        if (role == null)
        {
            throw new EntityNotFoundException(typeof(IdentityRole), id);
        }

        return role;
    }

    /// <summary>
    /// override Task<IdentityResult>
    /// </summary>
    public async override Task<IdentityResult> SetRoleNameAsync(IdentityRole role, string? name)
    {
        if (role.IsStatic && role.Name != name)
        {
            throw new BusinessException(IdentityErrorCodes.StaticRoleRenaming);
        }

        var userIdList = await UserRepository.GetUserIdListByRoleIdAsync(role.Id, cancellationToken: CancellationToken);
        var result = await base.SetRoleNameAsync(role, name);
        if (result.Succeeded)
        {
            Logger.LogDebug($"Remove dynamic claims cache for users of role: {role.Id}");
            await DynamicClaimCache.RemoveManyAsync(userIdList.Select(userId => AbpDynamicClaimCacheItem.CalculateCacheKey(userId, role.TenantId)), token: CancellationToken);
        }

        return result;
    }

    /// <summary>
    /// override Task<IdentityResult>
    /// </summary>
    public async override Task<IdentityResult> DeleteAsync(IdentityRole role)
    {
        if (role.IsStatic)
        {
            throw new BusinessException(IdentityErrorCodes.StaticRoleDeletion);
        }

        var userIdList = await UserRepository.GetUserIdListByRoleIdAsync(role.Id, cancellationToken: CancellationToken);
        var orgList = await OrganizationUnitRepository.GetListByRoleIdAsync(role.Id, includeDetails: false, cancellationToken: CancellationToken);
        var result = await base.DeleteAsync(role);
        if (result.Succeeded)
        {
            Logger.LogDebug($"Remove dynamic claims cache for users of role: {role.Id}");
            await DynamicClaimCache.RemoveManyAsync(userIdList.Select(userId => AbpDynamicClaimCacheItem.CalculateCacheKey(userId, role.TenantId)), token: CancellationToken);
            foreach (var organizationUnit in orgList)
            {
                await OrganizationUnitManager.RemoveDynamicClaimCacheAsync(organizationUnit);
            }
        }

        return result;
    }
}
