using System;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.LocalizationManagement;

public interface ILocalizationTextAppService : IApplicationService
{
    Task<PagedResultDto<LocalizationTextDto>> GetListAsync(GetLocalizationTextsInput input);

    Task<LocalizationTextDto> GetAsync(Guid id);

    Task<LocalizationTextDto> CreateAsync(CreateLocalizationTextDto input);

    Task<LocalizationTextDto> UpdateAsync(Guid id, UpdateLocalizationTextDto input);

    Task DeleteAsync(Guid id);
}
