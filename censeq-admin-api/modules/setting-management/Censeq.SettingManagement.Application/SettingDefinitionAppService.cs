using System;
using System.Linq;
using System.Threading.Tasks;
using Censeq.SettingManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Guids;

namespace Censeq.SettingManagement;

[Authorize(SettingManagementPermissions.SettingDefinitions.Default)]
public class SettingDefinitionAppService : SettingManagementAppServiceBase, ISettingDefinitionAppService
{
    private readonly ISettingDefinitionRecordRepository _definitionRepo;
    private readonly ISettingManagementStore _settingStore;
    private readonly ISettingRepository _settingRepo;
    private readonly IGuidGenerator _guidGenerator;

    public SettingDefinitionAppService(
        ISettingDefinitionRecordRepository definitionRepo,
        ISettingManagementStore settingStore,
        ISettingRepository settingRepo,
        IGuidGenerator guidGenerator)
    {
        _definitionRepo = definitionRepo;
        _settingStore = settingStore;
        _settingRepo = settingRepo;
        _guidGenerator = guidGenerator;
    }

    public virtual async Task<PagedResultDto<SettingDefinitionDto>> GetListAsync(SettingDefinitionGetListInput input)
    {
        var totalCount = await _definitionRepo.GetCountAsync(input.Filter);
        var definitions = await _definitionRepo.GetPagedListAsync(input.Filter, input.SkipCount, input.MaxResultCount);

        var names = definitions.Select(x => x.Name).ToList();
        var settings = await _settingRepo.GetListAsync(names.ToArray(), providerName: null, providerKey: null);

        var globalValueMap = settings
            .Where(x => x.ProviderName == SettingConsts.ProviderNames.Global)
            .ToDictionary(x => x.Name, x => x.Value);

        var dtos = definitions
            .Select(d => MapToDto(d, globalValueMap.GetValueOrDefault(d.Name)))
            .ToList();

        return new PagedResultDto<SettingDefinitionDto>(totalCount, dtos);
    }

    public virtual async Task<SettingDefinitionDto> GetAsync(Guid id)
    {
        var def = await _definitionRepo.GetAsync(id);
        var settings = await _settingRepo.GetListAsync(new[] { def.Name }, providerName: null, providerKey: null);
        var globalValue = settings.FirstOrDefault(x => x.ProviderName == SettingConsts.ProviderNames.Global)?.Value;
        return MapToDto(def, globalValue);
    }

    [Authorize(SettingManagementPermissions.SettingDefinitions.Create)]
    public virtual async Task<SettingDefinitionDto> CreateAsync(CreateSettingDefinitionDto input)
    {
        var existing = await _definitionRepo.FindByNameAsync(input.Name);
        if (existing != null)
            throw new UserFriendlyException($"配置编码 '{input.Name}' 已存在");

        var def = new SettingDefinitionRecord(
            _guidGenerator.Create(),
            input.Name,
            input.DisplayName,
            input.Description,
            input.DefaultValue,
            isVisibleToClients: input.IsVisibleToClients,
            providers: SettingConsts.ProviderNames.Global,
            isInherited: true,
            isEncrypted: false);

        await _definitionRepo.InsertAsync(def, autoSave: true);

        if (!input.CurrentValue.IsNullOrWhiteSpace())
        {
            await _settingStore.SetAsync(input.Name, input.CurrentValue!, SettingConsts.ProviderNames.Global, null);
        }

        return await GetAsync(def.Id);
    }

    [Authorize(SettingManagementPermissions.SettingDefinitions.Update)]
    public virtual async Task<SettingDefinitionDto> UpdateAsync(Guid id, UpdateSettingDefinitionDto input)
    {
        var def = await _definitionRepo.GetAsync(id);

        def.DisplayName = input.DisplayName;
        def.Description = input.Description;
        def.DefaultValue = input.DefaultValue;
        def.IsVisibleToClients = input.IsVisibleToClients;

        await _definitionRepo.UpdateAsync(def, autoSave: true);

        if (!input.CurrentValue.IsNullOrWhiteSpace())
        {
            await _settingStore.SetAsync(def.Name, input.CurrentValue!, SettingConsts.ProviderNames.Global, null);
        }
        else
        {
            await _settingStore.DeleteAsync(def.Name, SettingConsts.ProviderNames.Global, null);
        }

        return await GetAsync(id);
    }

    [Authorize(SettingManagementPermissions.SettingDefinitions.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var def = await _definitionRepo.GetAsync(id);

        if (def.LocalizationKey != null)
            throw new UserFriendlyException("系统内置配置不允许删除");

        var settings = await _settingRepo.GetListAsync(
            names: new[] { def.Name },
            providerName: null,
            providerKey: null);
        foreach (var s in settings)
            await _settingRepo.DeleteAsync(s);

        await _definitionRepo.DeleteAsync(id, autoSave: true);
    }

    private static SettingDefinitionDto MapToDto(SettingDefinitionRecord def, string? currentValue)
    {
        return new SettingDefinitionDto
        {
            Id = def.Id,
            Name = def.Name,
            DisplayName = def.DisplayName,
            Description = def.Description,
            DefaultValue = def.DefaultValue,
            CurrentValue = currentValue,
            IsVisibleToClients = def.IsVisibleToClients,
            IsSystem = def.LocalizationKey != null,
        };
    }
}
