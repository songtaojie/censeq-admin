using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

public interface IOrganizationUnitAppService : IApplicationService
{
    Task<OrganizationUnitDto> GetAsync(Guid id);

    Task<ListResultDto<OrganizationUnitDto>> GetAllListAsync();

    Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input);

    Task<OrganizationUnitDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input);

    Task DeleteAsync(Guid id);
}
