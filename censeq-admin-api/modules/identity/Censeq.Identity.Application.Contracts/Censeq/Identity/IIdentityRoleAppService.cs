using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

/// <summary>
/// 身份角色应用服务接口
/// </summary>
public interface IIdentityRoleAppService
    : ICrudAppService<
        IdentityRoleDto,
        Guid,
        GetIdentityRolesInput,
        IdentityRoleCreateDto,
        IdentityRoleUpdateDto>
{
    /// <summary>
    /// 获取所有角色列表
    /// </summary>
    Task<ListResultDto<IdentityRoleDto>> GetAllListAsync();

    /// <summary>
    /// 获取角色声明列表
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<IdentityRoleClaimListDto> GetClaimsAsync(Guid roleId);

    /// <summary>
    /// 添加角色声明
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    Task AddClaimAsync(Guid roleId, IdentityRoleClaimCreateDto input);

    /// <summary>
    /// 移除角色声明
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="claimId"></param>
    /// <returns></returns>
    Task RemoveClaimAsync(Guid roleId, Guid claimId);
}
