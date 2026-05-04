using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.LocalizationManagement;

public interface ILocalizationCultureAppService : IApplicationService
{
    Task<List<LocalizationCultureDto>> GetListAsync();

    Task<LocalizationCultureDto> CreateAsync(CreateUpdateLocalizationCultureDto input);

    Task<LocalizationCultureDto> UpdateAsync(Guid id, CreateUpdateLocalizationCultureDto input);

    Task DeleteAsync(Guid id);
}
