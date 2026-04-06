using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

public interface IIdentityRoleAppService
    : ICrudAppService<
        IdentityRoleDto,
        Guid,
        GetIdentityRolesInput,
        IdentityRoleCreateDto,
        IdentityRoleUpdateDto>
{
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
