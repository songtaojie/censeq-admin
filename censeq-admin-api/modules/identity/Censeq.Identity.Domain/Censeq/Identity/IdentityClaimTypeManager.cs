using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Censeq.Identity;

/// <summary>
/// 身份声明类型管理器
/// </summary>
public class IdentityClaimTypeManager : DomainService
{
    /// <summary>
    /// I身份声明类型仓储
    /// </summary>
    protected IIdentityClaimTypeRepository IdentityClaimTypeRepository { get; }
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository IdentityUserRepository { get; }
    /// <summary>
    /// I身份角色仓储
    /// </summary>
    protected IIdentityRoleRepository IdentityRoleRepository { get; }

    public IdentityClaimTypeManager(
        IIdentityClaimTypeRepository identityClaimTypeRepository,
        IIdentityUserRepository identityUserRepository,
        IIdentityRoleRepository identityRoleRepository)
    {
        IdentityClaimTypeRepository = identityClaimTypeRepository;
        IdentityUserRepository = identityUserRepository;
        IdentityRoleRepository = identityRoleRepository;
    }

    /// <summary>
    /// Task<Identity声明Type>
    /// </summary>
    public virtual async Task<IdentityClaimType> CreateAsync(IdentityClaimType claimType)
    {
        if (await IdentityClaimTypeRepository.AnyAsync(claimType.Name))
        {
            throw new BusinessException(IdentityErrorCodes.ClaimNameExist).WithData("0", claimType.Name);
        }

        return await IdentityClaimTypeRepository.InsertAsync(claimType);
    }

    /// <summary>
    /// Task<Identity声明Type>
    /// </summary>
    public virtual async Task<IdentityClaimType> UpdateAsync(IdentityClaimType claimType)
    {
        if (await IdentityClaimTypeRepository.AnyAsync(claimType.Name, claimType.Id))
        {
            throw new AbpException($"Name Exist: {claimType.Name}");
        }

        if (claimType.IsStatic)
        {
            throw new AbpException($"Can not update a static ClaimType.");
        }

        return await IdentityClaimTypeRepository.UpdateAsync(claimType);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var claimType = await IdentityClaimTypeRepository.GetAsync(id);
        if (claimType.IsStatic)
        {
            throw new AbpException($"Can not delete a static ClaimType.");
        }

        //Remove claim of this type from all users and roles
        await IdentityUserRepository.RemoveClaimFromAllUsersAsync(claimType.Name);
        await IdentityRoleRepository.RemoveClaimFromAllRolesAsync(claimType.Name);

        await IdentityClaimTypeRepository.DeleteAsync(id);
    }

    public virtual void ValidateValue(IdentityClaimType claimType, string claimValue)
    {
        if (claimType.Required && claimValue.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException($"声明值不能为空：{claimType.Name}");
        }

        if (claimValue.IsNullOrWhiteSpace())
        {
            return;
        }

        switch (claimType.ValueType)
        {
            case IdentityClaimValueType.Int:
                if (!int.TryParse(claimValue, out _))
                {
                    throw new UserFriendlyException($"声明值必须是整数：{claimType.Name}");
                }
                break;
            case IdentityClaimValueType.Boolean:
                if (!bool.TryParse(claimValue, out _))
                {
                    throw new UserFriendlyException($"声明值必须是布尔值：{claimType.Name}");
                }
                break;
            case IdentityClaimValueType.DateTime:
                if (!DateTime.TryParse(claimValue, out _))
                {
                    throw new UserFriendlyException($"声明值必须是日期时间：{claimType.Name}");
                }
                break;
            case IdentityClaimValueType.Option:
                if (!claimType.Options.Any(x => x.IsEnabled && x.Value == claimValue))
                {
                    throw new UserFriendlyException($"声明值不在可选项范围内：{claimType.Name}");
                }
                return;
        }

        if (!claimType.Regex.IsNullOrWhiteSpace() &&
            !Regex.IsMatch(claimValue, claimType.Regex))
        {
            throw new UserFriendlyException(claimType.RegexDescription.IsNullOrWhiteSpace()
                ? $"声明值格式不正确：{claimType.Name}"
                : claimType.RegexDescription);
        }
    }
}
