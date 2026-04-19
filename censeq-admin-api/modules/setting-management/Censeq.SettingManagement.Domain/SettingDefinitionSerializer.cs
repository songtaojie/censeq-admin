using Censeq.SettingManagement.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Censeq.SettingManagement;

public class SettingDefinitionSerializer : ISettingDefinitionSerializer, ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }
    protected IStringLocalizerFactory StringLocalizerFactory { get; }

    public SettingDefinitionSerializer(
        IGuidGenerator guidGenerator,
        ILocalizableStringSerializer localizableStringSerializer,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        GuidGenerator = guidGenerator;
        LocalizableStringSerializer = localizableStringSerializer;
        StringLocalizerFactory = stringLocalizerFactory;
    }

    /// <summary>将 ILocalizableString 解析为实际文本，回退到序列化 key</summary>
    private string? ResolveDisplayName(ILocalizableString? displayName, string? fallback = null)
    {
        if (displayName == null) return fallback;
        try
        {
            using (CultureHelper.Use(new CultureInfo("zh-Hans")))
            {
                var text = displayName.Localize(StringLocalizerFactory);
                if (!string.IsNullOrWhiteSpace(text)) return text;
            }
        }
        catch { }
        return LocalizableStringSerializer.Serialize(displayName) ?? fallback;
    }

    public virtual Task<SettingDefinitionRecord> SerializeAsync(SettingDefinition setting)
    {
        var localizationKey = LocalizableStringSerializer.Serialize(setting.DisplayName)!;
        var displayName = ResolveDisplayName(setting.DisplayName, setting.Name)!;
        var descriptionLocalizationKey = setting.Description != null
            ? LocalizableStringSerializer.Serialize(setting.Description)
            : null;
        var description = setting.Description != null
            ? ResolveDisplayName(setting.Description)
            : null;

        var record = new SettingDefinitionRecord(
            GuidGenerator.Create(),
            setting.Name,
            displayName,
            description,
            setting.DefaultValue,
            setting.IsVisibleToClients,
            SerializeProviders(setting.Providers),
            setting.IsInherited,
            setting.IsEncrypted,
            localizationKey,
            descriptionLocalizationKey);

        foreach (var property in setting.Properties)
        {
            record.SetProperty(property.Key, property.Value);
        }

        return Task.FromResult(record);
    }

    public virtual async Task<List<SettingDefinitionRecord>> SerializeAsync(IEnumerable<SettingDefinition> settings)
    {
        var records = new List<SettingDefinitionRecord>();
        foreach (var setting in settings)
        {
            records.Add(await SerializeAsync(setting));
        }

        return records;
    }

    protected virtual string? SerializeProviders(ICollection<string> providers)
    {
        return providers.Any() ? providers.JoinAsString(",") : null;
    }
}
