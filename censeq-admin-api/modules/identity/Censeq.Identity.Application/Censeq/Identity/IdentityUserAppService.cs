using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Censeq.TenantManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;

namespace Censeq.Identity;

/// <summary>
/// 身份用户应用服务
/// </summary>
public class IdentityUserAppService : IdentityAppServiceBase, IIdentityUserAppService
{
    /// <summary>
    /// 身份用户管理器
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository UserRepository { get; }
    /// <summary>
    /// I身份角色仓储
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// IOptions<IdentityOptions>
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions { get; }
    /// <summary>
    /// I权限Checker
    /// </summary>
    protected IPermissionChecker PermissionChecker { get; }
    protected ITenantRepository TenantRepository { get; }

    public IdentityUserAppService(
        IdentityUserManager userManager,
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IOptions<IdentityOptions> identityOptions,
        IPermissionChecker permissionChecker,
        ITenantRepository tenantRepository)
    {
        UserManager = userManager;
        UserRepository = userRepository;
        RoleRepository = roleRepository;
        IdentityOptions = identityOptions;
        PermissionChecker = permissionChecker;
        TenantRepository = tenantRepository;
    }

    //TODO: [Authorize(IdentityPermissions.Users.Default)] should go the IdentityUserAppService class.
    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<Identity用户Dto>
    /// </summary>
    public virtual async Task<IdentityUserDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(
            await UserManager.GetByIdAsync(id)
        );
    }

    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<Paged结果Dto<Identity用户Dto>>
    /// </summary>
    public virtual async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
    {
        var count = await UserRepository.GetCountAsync(input.Filter);
        var list = await UserRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);

        return new PagedResultDto<IdentityUserDto>(
            count,
            ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(list)
        );
    }

    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<List结果Dto<Identity角色Dto>>
    /// </summary>
    public virtual async Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id)
    {
        //TODO: Should also include roles of the related OUs.

        var roles = await UserRepository.GetRolesAsync(id);

        return new ListResultDto<IdentityRoleDto>(
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(roles)
        );
    }

    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<List结果Dto<Identity角色Dto>>
    /// </summary>
    public virtual async Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {
        var list = await RoleRepository.GetListAsync();
        return new ListResultDto<IdentityRoleDto>(
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list));
    }

    [Authorize(IdentityPermissions.Users.Create)]
    /// <summary>
    /// Task<Identity用户Dto>
    /// </summary>
    public virtual async Task<IdentityUserDto> CreateAsync(IdentityUserCreateDto input)
    {
        await IdentityOptions.SetAsync();
        await CheckTenantUserQuotaAsync();

        var user = new IdentityUser(
            GuidGenerator.Create(),
            input.UserName,
            input.Email,
            CurrentTenant.Id
        );

        input.MapExtraPropertiesTo(user);

        (await UserManager!.CreateAsync(user, input.Password))!.CheckErrors();
        await UpdateUserByInput(user, input);
        (await UserManager.UpdateAsync(user)).CheckErrors();

        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper!.Map<IdentityUser, IdentityUserDto>(user!);
    }

    protected virtual async Task CheckTenantUserQuotaAsync()
    {
        if (!CurrentTenant.Id.HasValue)
        {
            return;
        }

        var tenant = await TenantRepository.GetAsync(CurrentTenant.Id.Value);
        if (tenant.MaxUserCount <= 0)
        {
            return;
        }

        var currentUserCount = await UserRepository.GetCountAsync();
        if (currentUserCount >= tenant.MaxUserCount)
        {
            throw new BusinessException("Censeq.TenantManagement:TenantUserQuotaExceeded")
                .WithData("TenantId", tenant.Id)
                .WithData("MaxUserCount", tenant.MaxUserCount)
                .WithData("CurrentUserCount", currentUserCount);
        }
    }

    [Authorize(IdentityPermissions.Users.Update)]
    /// <summary>
    /// Task<Identity用户Dto>
    /// </summary>
    public virtual async Task<IdentityUserDto> UpdateAsync(Guid id, IdentityUserUpdateDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(id);
        if (user == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityUser), id);
        }

        user.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        (await UserManager.SetUserNameAsync(user, input.UserName)).CheckErrors();

        await UpdateUserByInput(user, input);
        input.MapExtraPropertiesTo(user);

        (await UserManager.UpdateAsync(user)).CheckErrors();

        if (!input.Password.IsNullOrEmpty())
        {
            (await UserManager.RemovePasswordAsync(user)).CheckErrors();
            (await UserManager.AddPasswordAsync(user, input.Password)).CheckErrors();
        }

        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper!.Map<IdentityUser, IdentityUserDto>(user!);
    }

    [Authorize(IdentityPermissions.Users.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        if (CurrentUser.Id == id)
        {
            throw new BusinessException(code: IdentityErrorCodes.UserSelfDeletion);
        }

        var user = await UserManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return;
        }

        (await UserManager.DeleteAsync(user)).CheckErrors();
    }

    [Authorize(IdentityPermissions.Users.Update)]
    public virtual async Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input)
    {
        await IdentityOptions.SetAsync();
        var user = await UserManager.GetByIdAsync(id);
        if (user == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityUser), id);
        }
        (await UserManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
        await UserRepository.UpdateAsync(user);
    }

    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<List结果Dto<Organization单元Dto>>
    /// </summary>
    public virtual async Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id)
    {
        var list = await UserRepository.GetOrganizationUnitsAsync(id, includeDetails: true);
        return new ListResultDto<OrganizationUnitDto>(
            ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(list));
    }

    [Authorize(IdentityPermissions.Users.Update)]
    public virtual async Task UpdateOrganizationUnitsAsync(Guid id, IdentityUserOrganizationUnitsDto input)
    {
        var user = await UserRepository.GetAsync(id, includeDetails: true);
        var target = new HashSet<Guid>(input.OrganizationUnitIds ?? new List<Guid>());
        var current = user.OrganizationUnits.Select(x => x.OrganizationUnitId).ToHashSet();

        foreach (var ouId in current.Except(target).ToList())
        {
            user.RemoveOrganizationUnit(ouId);
        }

        foreach (var ouId in target.Except(current).ToList())
        {
            user.AddOrganizationUnit(ouId);
        }

        await UserRepository.UpdateAsync(user);
    }

    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<Identity用户Dto>
    /// </summary>
    public virtual async Task<IdentityUserDto> FindByUsernameAsync(string userName)
    {
        var user = await UserManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityUser), userName);
        }
        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    [Authorize(IdentityPermissions.Users.Default)]
    /// <summary>
    /// Task<Identity用户Dto>
    /// </summary>
    public virtual async Task<IdentityUserDto> FindByEmailAsync(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityUser), email);
        }
        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    [Authorize(IdentityPermissions.Users.Update)]
    public virtual async Task ResetPasswordAsync(Guid id, IdentityUserResetPasswordDto input)
    {
        await IdentityOptions.SetAsync();
        var user = await UserManager.GetByIdAsync(id);
        if (user == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityUser), id);
        }
        (await UserManager.RemovePasswordAsync(user)).CheckErrors();
        (await UserManager.AddPasswordAsync(user, input.NewPassword)).CheckErrors();
        await CurrentUnitOfWork!.SaveChangesAsync();
    }

    protected virtual async Task UpdateUserByInput(IdentityUser user, IdentityUserCreateOrUpdateDtoBase input)
    {
        if (!string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
        {
            (await UserManager.SetEmailAsync(user, input.Email)).CheckErrors();
        }


        if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckErrors();
        }

        if (user.Id != CurrentUser.Id)
        {
            (await UserManager.SetLockoutEnabledAsync(user, input.LockoutEnabled)).CheckErrors();
        }

        user.Name = input.Name;
        user.Surname = input.Surname;
        (await UserManager.UpdateAsync(user)).CheckErrors();
        user.SetIsActive(input.IsActive);
        if (input.RoleNames != null && await PermissionChecker.IsGrantedAsync(IdentityPermissions.Users.ManageRoles))
        {
            (await UserManager.SetRolesAsync(user, input.RoleNames)).CheckErrors();
        }
    }
}
