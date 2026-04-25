using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

/// <summary>
/// 身份用户应用服务接口
/// </summary>
public interface IIdentityUserAppService
    : ICrudAppService<
        IdentityUserDto,
        Guid,
        GetIdentityUsersInput,
        IdentityUserCreateDto,
        IdentityUserUpdateDto>
{
    /// <summary>
    /// 获取用户角色列表
    /// </summary>
    Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id);

    /// <summary>
    /// 获取可分配的角色列表
    /// </summary>
    Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();

    /// <summary>
    /// 更新用户角色
    /// </summary>
    Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input);

    /// <summary>
    /// 获取用户所属组织单元列表
    /// </summary>
    Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id);

    /// <summary>
    /// 更新用户组织单元
    /// </summary>
    Task UpdateOrganizationUnitsAsync(Guid id, IdentityUserOrganizationUnitsDto input);

    /// <summary>
    /// 根据用户名查找用户
    /// </summary>
    Task<IdentityUserDto> FindByUsernameAsync(string userName);

    /// <summary>
    /// 根据邮箱查找用户
    /// </summary>
    Task<IdentityUserDto> FindByEmailAsync(string email);

    /// <summary>
    /// 重置用户密码
    /// </summary>
    Task ResetPasswordAsync(Guid id, IdentityUserResetPasswordDto input);
}
