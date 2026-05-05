using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.SettingManagement;

public interface ISettingDefinitionAppService : IApplicationService
{
    Task<PagedResultDto<SettingDefinitionDto>> GetListAsync(SettingDefinitionGetListInput input);

    Task<SettingDefinitionDto> GetAsync(Guid id);

    Task<SettingDefinitionDto> CreateAsync(CreateSettingDefinitionDto input);

    Task<SettingDefinitionDto> UpdateAsync(Guid id, UpdateSettingDefinitionDto input);

    Task DeleteAsync(Guid id);
}
