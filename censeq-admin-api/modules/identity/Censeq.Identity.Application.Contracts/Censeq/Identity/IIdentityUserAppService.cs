using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

public interface IIdentityUserAppService
    : ICrudAppService<
        IdentityUserDto,
        Guid,
        GetIdentityUsersInput,
        IdentityUserCreateDto,
        IdentityUserUpdateDto>
{
    Task<ListResultDto<IdentityRoleDto>> GetRolesAsync(Guid id);

    Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync();

    Task UpdateRolesAsync(Guid id, IdentityUserUpdateRolesDto input);

    Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnitsAsync(Guid id);

    Task UpdateOrganizationUnitsAsync(Guid id, IdentityUserOrganizationUnitsDto input);

    Task<IdentityUserDto> FindByUsernameAsync(string userName);

    Task<IdentityUserDto> FindByEmailAsync(string email);
}
