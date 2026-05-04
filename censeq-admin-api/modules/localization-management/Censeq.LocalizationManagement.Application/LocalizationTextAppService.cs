using System;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Censeq.LocalizationManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.LocalizationManagement;

[Authorize(LocalizationManagementPermissions.Texts.Default)]
public class LocalizationTextAppService : ApplicationService, ILocalizationTextAppService
{
    private readonly ILocalizationTextRepository _textRepository;

    public LocalizationTextAppService(ILocalizationTextRepository textRepository)
    {
        _textRepository = textRepository;
    }

    public async Task<PagedResultDto<LocalizationTextDto>> GetListAsync(GetLocalizationTextsInput input)
    {
        var count = await _textRepository.GetCountAsync(
            input.ResourceName, input.CultureName, CurrentTenant.Id, input.Filter);

        var items = await _textRepository.GetPagedListAsync(
            input.ResourceName, input.CultureName, CurrentTenant.Id,
            input.Filter, input.SkipCount, input.MaxResultCount, input.Sorting);

        return new PagedResultDto<LocalizationTextDto>(
            count,
            ObjectMapper.Map<System.Collections.Generic.List<LocalizationText>, System.Collections.Generic.List<LocalizationTextDto>>(items));
    }

    public async Task<LocalizationTextDto> GetAsync(Guid id)
    {
        var text = await _textRepository.GetAsync(id);
        return ObjectMapper.Map<LocalizationText, LocalizationTextDto>(text);
    }

    [Authorize(LocalizationManagementPermissions.Texts.Create)]
    public async Task<LocalizationTextDto> CreateAsync(CreateLocalizationTextDto input)
    {
        var existing = await _textRepository.FindAsync(
            input.ResourceName, input.CultureName, input.Key, CurrentTenant.Id);

        if (existing != null)
        {
            throw new UserFriendlyException(
                $"Translation already exists: [{input.ResourceName}] [{input.CultureName}] {input.Key}");
        }

        var text = new LocalizationText(
            GuidGenerator.Create(),
            input.ResourceName,
            input.CultureName,
            input.Key,
            input.Value,
            CurrentTenant.Id);

        await _textRepository.InsertAsync(text, autoSave: true);
        return ObjectMapper.Map<LocalizationText, LocalizationTextDto>(text);
    }

    [Authorize(LocalizationManagementPermissions.Texts.Update)]
    public async Task<LocalizationTextDto> UpdateAsync(Guid id, UpdateLocalizationTextDto input)
    {
        var text = await _textRepository.GetAsync(id);
        text.Value = input.Value;

        await _textRepository.UpdateAsync(text, autoSave: true);
        return ObjectMapper.Map<LocalizationText, LocalizationTextDto>(text);
    }

    [Authorize(LocalizationManagementPermissions.Texts.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _textRepository.DeleteAsync(id, autoSave: true);
    }
}
