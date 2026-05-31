using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityRole = Censeq.Identity.Entities.IdentityRole;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;
using Volo.Abp;

namespace Censeq.Identity;

[Authorize(IdentityPermissions.Roles.Default)]
/// <summary>
/// 韬唤瑙掕壊搴旂敤鏈嶅姟
/// </summary>
public class IdentityRoleAppService : IdentityAppServiceBase, IIdentityRoleAppService
{
    /// <summary>
    /// 韬唤瑙掕壊绠＄悊鍣?
    /// </summary>
    protected IdentityRoleManager RoleManager { get; }
    /// <summary>
    /// I韬唤瑙掕壊浠撳偍
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// I韬唤澹版槑绫诲瀷浠撳偍
    /// </summary>
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
    protected IdentityClaimTypeManager ClaimTypeManager { get; }

    public IdentityRoleAppService(
        IdentityRoleManager roleManager,
        IIdentityRoleRepository roleRepository,
        IIdentityClaimTypeRepository claimTypeRepository,
        IdentityClaimTypeManager claimTypeManager)
    {
        RoleManager = roleManager;
        RoleRepository = roleRepository;
        ClaimTypeRepository = claimTypeRepository;
        ClaimTypeManager = claimTypeManager;
    }

    /// <summary>
    /// Task<Identity瑙掕壊Dto>
    /// </summary>
    public virtual async Task<IdentityRoleDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<IdentityRole, IdentityRoleDto>(
            await RoleManager.GetByIdAsync(id)
        );
    }

    /// <summary>
    /// Task<List缁撴灉Dto<Identity瑙掕壊Dto>>
    /// </summary>
    public virtual async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {
        var list = await RoleRepository.GetListAsync();
        return new ListResultDto<IdentityRoleDto>(
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(list)
        );
    }

    /// <summary>
    /// Task<Paged缁撴灉Dto<Identity瑙掕壊Dto>>
    /// </summary>
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
    /// <summary>
    /// Task<Identity瑙掕壊Dto>
    /// </summary>
    public virtual async Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
    {
        var normalizedCode = NormalizeRoleCode(input.Code);
        await EnsureRoleCodeUniqueAsync(normalizedCode);

        var role = new IdentityRole(
            GuidGenerator.Create(),
            input.Name,
            CurrentTenant.Id
        )
        {
            IsDefault = input.IsDefault,
            IsPublic = input.IsPublic,
            Status = input.Status,
            Remark = input.Remark
        };

        role.SetCode(normalizedCode);

        input.MapExtraPropertiesTo(role);

        (await RoleManager!.CreateAsync(role!))!.CheckErrors();
        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper!.Map<IdentityRole, IdentityRoleDto>(role!);
    }

    [Authorize(IdentityPermissions.Roles.Update)]
    /// <summary>
    /// Task<Identity瑙掕壊Dto>
    /// </summary>
    public virtual async Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
    {
        var role = await RoleManager.GetByIdAsync(id);
        if (role == null)
        {
            throw new EntityNotFoundException(typeof(IdentityRole), id);
        }

        role.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

        if (!string.Equals(role.Name, input.Name, StringComparison.Ordinal))
        {
            (await RoleManager.SetRoleNameAsync(role, input.Name)).CheckErrors();
            // SetRoleNameAsync 宸茬粡鏇存柊浜嗚鑹诧紝闇€瑕侀噸鏂拌幏鍙栦互鏇存柊 ConcurrencyStamp
            role = await RoleManager.GetByIdAsync(id);
        }

        role.IsDefault = input.IsDefault;
        role.IsPublic = input.IsPublic;
        role.Status = input.Status;
        role.Remark = input.Remark;

        var normalizedCode = NormalizeRoleCode(input.Code);
        if (!string.Equals(role.Code, normalizedCode, StringComparison.Ordinal))
        {
            if (!string.IsNullOrWhiteSpace(role.Code))
            {
                throw new UserFriendlyException("角色编码已设置，不允许再次修改。");
            }

            await EnsureRoleCodeUniqueAsync(normalizedCode, id);
            role.SetCode(normalizedCode);
        }

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
    /// <summary>
    /// Task<Identity瑙掕壊澹版槑鍒楄〃Dto>
    /// </summary>
    public virtual async Task<IdentityRoleClaimListDto> GetClaimsAsync(Guid roleId)
    {
        var role = await RoleRepository.FindByIdAsync(roleId, includeDetails: true);
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
        var claimType = await ClaimTypeRepository.FindByNameAsync(input.ClaimType, includeOptions: true);
        if (claimType == null)
        {
            throw new UserFriendlyException($"声明类型 '{input.ClaimType}' 不存在，请先在声明类型管理中维护。");
        }

        ClaimTypeManager.ValidateValue(claimType, input.ClaimValue);

        var role = await RoleRepository.FindByIdAsync(roleId, includeDetails: true);
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
        var role = await RoleRepository.FindByIdAsync(roleId, includeDetails: true);
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

    protected virtual async Task EnsureRoleCodeUniqueAsync(string code, Guid? excludedRoleId = null)
    {
        var existingRole = await RoleRepository.FindByCodeAsync(code,false);
        if (existingRole != null && existingRole.Id != excludedRoleId)
        {
            throw new UserFriendlyException($"角色编码“{code}”已存在，请使用其他编码。");
        }
    }

    protected virtual string NormalizeRoleCode(string? code)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("角色编码不能为空。");
        }

        return code.Trim().ToUpperInvariant();
    }
}
