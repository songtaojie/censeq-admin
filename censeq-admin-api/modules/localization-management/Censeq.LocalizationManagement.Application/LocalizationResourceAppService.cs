using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Censeq.LocalizationManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;

namespace Censeq.LocalizationManagement;

[Authorize(LocalizationManagementPermissions.Resources.Default)]
public class LocalizationResourceAppService : ApplicationService, ILocalizationResourceAppService
{
    private readonly ILocalizationResourceRepository _resourceRepository;

    public LocalizationResourceAppService(ILocalizationResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async Task<List<LocalizationResourceDto>> GetAllAsync()
    {
        var resources = await _resourceRepository.GetAllAsync();
        return ObjectMapper.Map<List<LocalizationResource>, List<LocalizationResourceDto>>(resources);
    }

    [Authorize(LocalizationManagementPermissions.Resources.Create)]
    public async Task<LocalizationResourceDto> CreateAsync(CreateUpdateLocalizationResourceDto input)
    {
        var existing = await _resourceRepository.FindByNameAsync(input.Name);
        if (existing != null)
            throw new Volo.Abp.UserFriendlyException($"资源 '{input.Name}' 已存在");

        var resource = new LocalizationResource(
            GuidGenerator.Create(),
            name: input.Name,
            displayName: input.DisplayName,
            defaultCultureName: input.DefaultCultureName);

        await _resourceRepository.InsertAsync(resource, autoSave: true);
        return ObjectMapper.Map<LocalizationResource, LocalizationResourceDto>(resource);
    }

    [Authorize(LocalizationManagementPermissions.Resources.Update)]
    public async Task<LocalizationResourceDto> UpdateAsync(Guid id, CreateUpdateLocalizationResourceDto input)
    {
        var resource = await _resourceRepository.GetAsync(id);
        resource.DisplayName = input.DisplayName;
        resource.DefaultCultureName = input.DefaultCultureName;

        await _resourceRepository.UpdateAsync(resource, autoSave: true);
        return ObjectMapper.Map<LocalizationResource, LocalizationResourceDto>(resource);
    }

    [Authorize(LocalizationManagementPermissions.Resources.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _resourceRepository.DeleteAsync(id, autoSave: true);
    }
}
