using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity;

public interface IIdentityRoleAppService
    : ICrudAppService<
        IdentityRoleDto,
        Guid,
        GetIdentityRolesInput,
        IdentityRoleCreateDto,
        IdentityRoleUpdateDto>
{
    Task<ListResultDto<IdentityRoleDto>> GetAllListAsync();
}
