using System;
using System.Text.Json.Serialization;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Censeq.FeatureManagement.Entities;

public class FeatureDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string GroupName { get; set; }

    public string Name  { get; set; }

    public string ParentName { get; set; }

    public string DisplayName  { get; set; }

    public string Description { get; set; }

    /// <summary>系统原始多语言 key（DisplayName），格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }

    /// <summary>系统原始多语言 key（Description），格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? DescriptionLocalizationKey { get; set; }

    public string DefaultValue { get; set; }

    public bool IsVisibleToClients { get; set; }

    public bool IsAvailableToHost { get; set; }

    /// <summary>
    /// Comma separated list of provider names.
    /// </summary>
    public string AllowedProviders { get; set; }

    /// <summary>
    /// Serialized string to store info about the ValueType.
    /// </summary>
    public string ValueType { get; set; } // ToggleStringValueType

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public FeatureDefinitionRecord()
    {
        IsVisibleToClients = true;
        IsAvailableToHost = true;
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public FeatureDefinitionRecord(
        Guid id,
        string groupName,
        string name,
        string? parentName,
        string? displayName = null,
        string? description = null,
        string? defaultValue = null,
        bool isVisibleToClients = true,
        bool isAvailableToHost = true,
        string? allowedProviders = null,
        string? valueType = null)
        : base(id)
    {
        GroupName = Check.NotNullOrWhiteSpace(groupName, nameof(groupName), FeatureDefinitionRecordConsts.MaxNameLength);
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), FeatureDefinitionRecordConsts.MaxNameLength);
        ParentName = Check.Length(parentName, nameof(parentName), FeatureDefinitionRecordConsts.MaxNameLength) ?? string.Empty;
        DisplayName = Check.NotNullOrWhiteSpace(displayName ?? string.Empty, nameof(displayName), FeatureDefinitionRecordConsts.MaxDisplayNameLength);

        Description = Check.Length(description, nameof(description), FeatureDefinitionRecordConsts.MaxDescriptionLength) ?? string.Empty;
        DefaultValue = Check.Length(defaultValue, nameof(defaultValue), FeatureDefinitionRecordConsts.MaxDefaultValueLength) ?? string.Empty;

        IsVisibleToClients = isVisibleToClients;
        IsAvailableToHost = isAvailableToHost;

        AllowedProviders = Check.Length(allowedProviders, nameof(allowedProviders), FeatureDefinitionRecordConsts.MaxAllowedProvidersLength) ?? string.Empty;
        ValueType = Check.NotNullOrWhiteSpace(valueType ?? string.Empty, nameof(valueType), FeatureDefinitionRecordConsts.MaxValueTypeLength);

        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }
    public bool HasSameData(FeatureDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name) return false;
        if (GroupName != otherRecord.GroupName) return false;
        if (ParentName != otherRecord.ParentName) return false;
        // DisplayName/Description 是用户可编辑字段，不参与系统同步的变更检测
        if (LocalizationKey != otherRecord.LocalizationKey) return false;
        if (DescriptionLocalizationKey != otherRecord.DescriptionLocalizationKey) return false;
        if (DefaultValue != otherRecord.DefaultValue) return false;
        if (IsVisibleToClients != otherRecord.IsVisibleToClients) return false;
        if (IsAvailableToHost != otherRecord.IsAvailableToHost) return false;
        if (AllowedProviders != otherRecord.AllowedProviders) return false;
        if (ValueType != otherRecord.ValueType) return false;
        if (!this.HasSameExtraProperties(otherRecord)) return false;
        return true;
    }

    public void Patch(FeatureDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name) Name = otherRecord.Name;
        if (GroupName != otherRecord.GroupName) GroupName = otherRecord.GroupName;
        if (ParentName != otherRecord.ParentName) ParentName = otherRecord.ParentName;
        // DisplayName/Description 是用户可编辑字段，系统同步时不覆盖
        if (LocalizationKey != otherRecord.LocalizationKey) LocalizationKey = otherRecord.LocalizationKey;
        if (DescriptionLocalizationKey != otherRecord.DescriptionLocalizationKey) DescriptionLocalizationKey = otherRecord.DescriptionLocalizationKey;
        if (DefaultValue != otherRecord.DefaultValue) DefaultValue = otherRecord.DefaultValue;
        if (IsVisibleToClients != otherRecord.IsVisibleToClients) IsVisibleToClients = otherRecord.IsVisibleToClients;
        if (IsAvailableToHost != otherRecord.IsAvailableToHost) IsAvailableToHost = otherRecord.IsAvailableToHost;
        if (AllowedProviders != otherRecord.AllowedProviders) AllowedProviders = otherRecord.AllowedProviders;
        if (ValueType != otherRecord.ValueType) ValueType = otherRecord.ValueType;
        if (!this.HasSameExtraProperties(otherRecord))
        {
            ExtraProperties.Clear();
            foreach (var property in otherRecord.ExtraProperties)
            {
                ExtraProperties.Add(property.Key, property.Value);
            }
        }
    }
}
