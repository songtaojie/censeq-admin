using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.LocalizationManagement;

public interface ILocalizationResourceAppService : IApplicationService
{
    Task<List<LocalizationResourceDto>> GetAllAsync();

    Task<LocalizationResourceDto> CreateAsync(CreateUpdateLocalizationResourceDto input);

    Task<LocalizationResourceDto> UpdateAsync(Guid id, CreateUpdateLocalizationResourceDto input);

    Task DeleteAsync(Guid id);
}
