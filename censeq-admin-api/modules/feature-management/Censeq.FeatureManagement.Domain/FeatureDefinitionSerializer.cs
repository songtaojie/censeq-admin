using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Censeq.FeatureManagement.Entities;
using Microsoft.Extensions.Localization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.Validation.StringValues;

namespace Censeq.FeatureManagement;

public class FeatureDefinitionSerializer : IFeatureDefinitionSerializer, ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }
    protected StringValueTypeSerializer StringValueTypeSerializer { get; }
    protected IStringLocalizerFactory StringLocalizerFactory { get; }

    public FeatureDefinitionSerializer(
        IGuidGenerator guidGenerator,
        ILocalizableStringSerializer localizableStringSerializer,
        StringValueTypeSerializer stringValueTypeSerializer,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        GuidGenerator = guidGenerator;
        LocalizableStringSerializer = localizableStringSerializer;
        StringValueTypeSerializer = stringValueTypeSerializer;
        StringLocalizerFactory = stringLocalizerFactory;
    }

    /// <summary>将 ILocalizableString 解析为实际文本，回退到序列化 key</summary>
    private string ResolveDisplayName(ILocalizableString? displayName, string fallback)
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

    public async Task<(FeatureGroupDefinitionRecord[], FeatureDefinitionRecord[])> SerializeAsync(IEnumerable<FeatureGroupDefinition> featureGroups)
    {
        var featureGroupRecords = new List<FeatureGroupDefinitionRecord>();
        var featureRecords = new List<FeatureDefinitionRecord>();

        foreach (var featureGroup in featureGroups)
        {
            featureGroupRecords.Add(await SerializeAsync(featureGroup));

            foreach (var feature in featureGroup.GetFeaturesWithChildren())
            {
                featureRecords.Add(await SerializeAsync(feature, featureGroup));
            }
        }

        return (featureGroupRecords.ToArray(), featureRecords.ToArray());
    }

    public Task<FeatureGroupDefinitionRecord> SerializeAsync(FeatureGroupDefinition featureGroup)
    {
        var localizationKey = LocalizableStringSerializer.Serialize(featureGroup.DisplayName) ?? string.Empty;
        var displayName = ResolveDisplayName(featureGroup.DisplayName, featureGroup.Name);

        var featureGroupRecord = new FeatureGroupDefinitionRecord(
            GuidGenerator.Create(),
            featureGroup.Name,
            displayName,
            localizationKey);

        foreach (var property in featureGroup.Properties)
        {
            featureGroupRecord.SetProperty(property.Key, property.Value);
        }

        return Task.FromResult(featureGroupRecord);
    }

    public Task<FeatureDefinitionRecord> SerializeAsync(FeatureDefinition feature, FeatureGroupDefinition featureGroup)
    {
        var localizationKey = LocalizableStringSerializer.Serialize(feature.DisplayName!) ?? string.Empty;
        var displayName = ResolveDisplayName(feature.DisplayName, feature.Name);
        var descriptionLocalizationKey = feature.Description != null
            ? LocalizableStringSerializer.Serialize(feature.Description)
            : null;
        var description = feature.Description != null
            ? ResolveDisplayName(feature.Description, string.Empty)
            : null;

        var featureRecord = new FeatureDefinitionRecord(
            GuidGenerator.Create(),
            featureGroup?.Name ?? string.Empty,
            feature.Name,
            feature.Parent?.Name,
            displayName,
            description,
            feature.DefaultValue,
            feature.IsVisibleToClients,
            feature.IsAvailableToHost,
            SerializeProviders(feature.AllowedProviders),
            SerializeStringValueType(feature.ValueType));

        featureRecord.LocalizationKey = localizationKey;
        featureRecord.DescriptionLocalizationKey = descriptionLocalizationKey;

        foreach (var property in feature.Properties)
        {
            featureRecord.SetProperty(property.Key, property.Value);
        }

        return Task.FromResult(featureRecord);
    }

    protected virtual string? SerializeProviders(ICollection<string> providers)
    {
        return providers.Any() ? providers.JoinAsString(",") : null;
    }

    protected virtual string? SerializeStringValueType(IStringValueType? stringValueType)
    {
        return stringValueType == null ? null : StringValueTypeSerializer.Serialize(stringValueType);
    }
}
