using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Censeq.SettingManagement;

[Authorize(SettingManagementPermissions.SettingDefinitions.Default)]
public class SettingValueAppService : SettingManagementAppServiceBase, ISettingValueAppService
{
    private readonly ISettingRepository _settingRepo;
    private readonly ISettingManagementStore _settingStore;

    public SettingValueAppService(
        ISettingRepository settingRepo,
        ISettingManagementStore settingStore)
    {
        _settingRepo = settingRepo;
        _settingStore = settingStore;
    }

    public virtual async Task<SettingValueDto> GetAsync(string name)
    {
        var settings = await _settingRepo.GetListAsync(
            names: new[] { name },
            providerName: null,
            providerKey: null);

        var globalValue = settings.FirstOrDefault(x => x.ProviderName == SettingConsts.ProviderNames.Global)?.Value;

        var tenantValues = settings
            .Where(x => x.ProviderName == SettingConsts.ProviderNames.Tenant && x.ProviderKey != null)
            .Select(x => new TenantSettingValueDto
            {
                TenantId = Guid.Parse(x.ProviderKey!),
                Value = x.Value
            })
            .ToList();

        var userValues = settings
            .Where(x => x.ProviderName == SettingConsts.ProviderNames.User && x.ProviderKey != null)
            .Select(x => new UserSettingValueDto
            {
                UserId = Guid.Parse(x.ProviderKey!),
                Value = x.Value
            })
            .ToList();

        return new SettingValueDto
        {
            Name = name,
            GlobalValue = globalValue,
            TenantValues = tenantValues,
            UserValues = userValues
        };
    }

    [Authorize(SettingManagementPermissions.SettingDefinitions.Update)]
    public virtual async Task SetAsync(SetSettingValueInput input)
    {
        if (string.IsNullOrEmpty(input.Value))
        {
            await _settingStore.DeleteAsync(input.Name, input.ProviderName, input.ProviderKey);
        }
        else
        {
            await _settingStore.SetAsync(input.Name, input.Value, input.ProviderName, input.ProviderKey);
        }
    }

    [Authorize(SettingManagementPermissions.SettingDefinitions.Update)]
    public virtual async Task DeleteAsync(string name, string providerName, string? providerKey)
    {
        await _settingStore.DeleteAsync(name, providerName, providerKey);
    }
}
