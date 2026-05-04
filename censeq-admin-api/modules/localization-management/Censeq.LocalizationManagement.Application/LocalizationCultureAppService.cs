using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Censeq.LocalizationManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Censeq.LocalizationManagement;

[Authorize(LocalizationManagementPermissions.Cultures.Default)]
public class LocalizationCultureAppService : ApplicationService, ILocalizationCultureAppService
{
    private readonly ILocalizationCultureRepository _cultureRepository;

    public LocalizationCultureAppService(ILocalizationCultureRepository cultureRepository)
    {
        _cultureRepository = cultureRepository;
    }

    public async Task<List<LocalizationCultureDto>> GetListAsync()
    {
        var cultures = await _cultureRepository.GetListAsync(CurrentTenant.Id);
        return ObjectMapper.Map<List<LocalizationCulture>, List<LocalizationCultureDto>>(cultures);
    }

    [Authorize(LocalizationManagementPermissions.Cultures.Create)]
    public async Task<LocalizationCultureDto> CreateAsync(CreateUpdateLocalizationCultureDto input)
    {
        var culture = new LocalizationCulture(
            GuidGenerator.Create(),
            input.CultureName,
            input.DisplayName,
            input.IsEnabled,
            input.UiCultureName,
            CurrentTenant.Id);

        await _cultureRepository.InsertAsync(culture, autoSave: true);
        return ObjectMapper.Map<LocalizationCulture, LocalizationCultureDto>(culture);
    }

    [Authorize(LocalizationManagementPermissions.Cultures.Update)]
    public async Task<LocalizationCultureDto> UpdateAsync(Guid id, CreateUpdateLocalizationCultureDto input)
    {
        var culture = await _cultureRepository.GetAsync(id);
        culture.DisplayName = input.DisplayName;
        culture.UiCultureName = input.UiCultureName;
        culture.IsEnabled = input.IsEnabled;

        await _cultureRepository.UpdateAsync(culture, autoSave: true);
        return ObjectMapper.Map<LocalizationCulture, LocalizationCultureDto>(culture);
    }

    [Authorize(LocalizationManagementPermissions.Cultures.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _cultureRepository.DeleteAsync(id, autoSave: true);
    }
}
