using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityRole = Censeq.Identity.Entities.IdentityRole;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp;

namespace Censeq.Identity;

[Authorize(IdentityPermissions.Roles.Default)]
public class IdentityRoleAppService : IdentityAppServiceBase, IIdentityRoleAppService
{
    protected IdentityRoleManager RoleManager { get; }
    protected IIdentityRoleRepository RoleRepository { get; }
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }

    public IdentityRoleAppService(
        IdentityRoleManager roleManager,
        IIdentityRoleRepository roleRepository,
        IIdentityClaimTypeRepository claimTypeRepository)
    {
        RoleManager = roleManager;
        RoleRepository = roleRepository;
        ClaimTypeRepository = claimTypeRepository;
    }

    public virtual async Task<IdentityRoleDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(
            await RoleManager.GetByIdAsync(id)
        );
    }

    public virtual async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {
        var list = await RoleRepository.GetListAsync();
        return new ListResultDto<IdentityRoleDto>(
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list)
        );
    }

    public virtual async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
    {
        var list = await RoleRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);
        var totalCount = await RoleRepository.GetCountAsync(input.Filter);

        return new PagedResultDto<IdentityRoleDto>(
            totalCount,
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list)
            );
    }

    [Authorize(IdentityPermissions.Roles.Create)]
    public virtual async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
    {
        var role = new IdentityRole(
            GuidGenerator.Create(),
            input.Name,
            CurrentTenant.Id
        )
        {
            IsDefault = input.IsDefault,
            IsPublic = input.IsPublic
        };

        input.MapExtraPropertiesTo(role);

        (await RoleManager!.CreateAsync(role!))!.CheckErrors();
        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper!.Map<IdentityRole, IdentityRoleDto>(role!);
    }

    [Authorize(IdentityPermissions.Roles.Update)]
    public virtual async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
    {
        var role = await RoleManager.GetByIdAsync(id);
        if (role == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityRole), id);
        }

        role.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        if (role.Name != input.Name)
        {
            (await RoleManager.SetRoleNameAsync(role, input.Name)).CheckErrors();
            // SetRoleNameAsync 已经更新了角色，需要重新获取以更新 ConcurrencyStamp
            role = await RoleManager.GetByIdAsync(id);
        }

        role.IsDefault = input.IsDefault;
        role.IsPublic = input.IsPublic;

        input.MapExtraPropertiesTo(role);

        (await RoleManager.UpdateAsync(role)).CheckErrors();

        return ObjectMapper!.Map<IdentityRole, IdentityRoleDto>(role!);
    }

    [Authorize(IdentityPermissions.Roles.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var role = await RoleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return;
        }

        (await RoleManager.DeleteAsync(role)).CheckErrors();
    }

    [Authorize(IdentityPermissions.Roles.Default)]
    public virtual async Task<IdentityRoleClaimListDto> GetClaimsAsync(Guid roleId)
    {
        var role = await RoleRepository.FindByNormalizedNameAsync(
            (await RoleManager.GetByIdAsync(roleId)).NormalizedName,
            includeDetails: true);
        if (role == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityRole), roleId);
        }

        var claims = role.Claims.Select(c => new IdentityRoleClaimDto
        {
            Id = c.Id,
            ClaimType = c.ClaimType,
            ClaimValue = c.ClaimValue
        }).ToList();

        return new IdentityRoleClaimListDto(claims);
    }

    [Authorize(IdentityPermissions.Roles.Update)]
    public virtual async Task AddClaimAsync(Guid roleId, IdentityRoleClaimCreateDto input)
    {
        if (!await ClaimTypeRepository.AnyAsync(input.ClaimType))
        {
            throw new UserFriendlyException($"声明类型 '{input.ClaimType}' 不存在，请先在声明类型管理中维护。");
        }

        var role = await RoleRepository.FindByNormalizedNameAsync(
            (await RoleManager.GetByIdAsync(roleId)).NormalizedName,
            includeDetails: true);
        if (role == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityRole), roleId);
        }

        var claim = new System.Security.Claims.Claim(input.ClaimType, input.ClaimValue);
        role.AddClaim(GuidGenerator, claim);

        await RoleRepository.UpdateAsync(role);
    }

    [Authorize(IdentityPermissions.Roles.Update)]
    public virtual async Task RemoveClaimAsync(Guid roleId, Guid claimId)
    {
        var role = await RoleRepository.FindByNormalizedNameAsync(
            (await RoleManager.GetByIdAsync(roleId)).NormalizedName,
            includeDetails: true);
        if (role == null)
        {
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(IdentityRole), roleId);
        }

        var claim = role.Claims.FirstOrDefault(c => c.Id == claimId);
        if (claim != null)
        {
            role.Claims.Remove(claim);
            await RoleRepository.UpdateAsync(role);
        }
    }
}
