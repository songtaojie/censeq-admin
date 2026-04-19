using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Localization;

namespace Censeq.SettingManagement.Entities;

public class SettingDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    /// <summary>Unique name of the setting.</summary>
    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public string DisplayName { get; set; }

    /// <summary>系统原始多语言 key（DisplayName），格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }

    [CanBeNull]
    public string? Description { get; set; }

    /// <summary>系统原始多语言 key（Description），格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? DescriptionLocalizationKey { get; set; }

    /// <summary>Default value of the setting.</summary>
    [CanBeNull]
    public string? DefaultValue { get; set; }

    /// <summary>Can clients see this setting and it's value. Default: false.</summary>
    public bool IsVisibleToClients { get; set; }

    /// <summary>Comma separated list of provider names.</summary>
    public string? Providers { get; set; }

    /// <summary>Is this setting inherited from parent scopes. Default: True.</summary>
    public bool IsInherited { get; set; }

    /// <summary>Is this setting stored as encrypted in the data source. Default: False.</summary>
    public bool IsEncrypted { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public SettingDefinitionRecord()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public SettingDefinitionRecord(
        Guid id,
        string name,
        string displayName,
        string? description,
        string? defaultValue,
        bool isVisibleToClients,
        string? providers,
        bool isInherited,
        bool isEncrypted,
        string? localizationKey = null,
        string? descriptionLocalizationKey = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), SettingDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), SettingDefinitionRecordConsts.MaxDisplayNameLength);
        Description = Check.Length(description, nameof(description), SettingDefinitionRecordConsts.MaxDescriptionLength);
        DefaultValue = Check.Length(defaultValue, nameof(defaultValue), SettingDefinitionRecordConsts.MaxDefaultValueLength);
        IsVisibleToClients = isVisibleToClients;
        Providers = Check.Length(providers, nameof(providers), SettingDefinitionRecordConsts.MaxProvidersLength);
        IsInherited = isInherited;
        IsEncrypted = isEncrypted;
        LocalizationKey = localizationKey;
        DescriptionLocalizationKey = descriptionLocalizationKey;
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(SettingDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name) return false;
        // DisplayName/Description 是用户可编辑字段，不参与系统同步的变更检测
        if (LocalizationKey != otherRecord.LocalizationKey) return false;
        if (DescriptionLocalizationKey != otherRecord.DescriptionLocalizationKey) return false;
        if (DefaultValue != otherRecord.DefaultValue) return false;
        if (IsVisibleToClients != otherRecord.IsVisibleToClients) return false;
        if (Providers != otherRecord.Providers) return false;
        if (IsInherited != otherRecord.IsInherited) return false;
        if (IsEncrypted != otherRecord.IsEncrypted) return false;
        if (!this.HasSameExtraProperties(otherRecord)) return false;
        return true;
    }

    public void Patch(SettingDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name) Name = otherRecord.Name;
        // DisplayName/Description 是用户可编辑字段，系统同步时不覆盖
        if (LocalizationKey != otherRecord.LocalizationKey) LocalizationKey = otherRecord.LocalizationKey;
        if (DescriptionLocalizationKey != otherRecord.DescriptionLocalizationKey) DescriptionLocalizationKey = otherRecord.DescriptionLocalizationKey;
        if (DefaultValue != otherRecord.DefaultValue) DefaultValue = otherRecord.DefaultValue;
        if (IsVisibleToClients != otherRecord.IsVisibleToClients) IsVisibleToClients = otherRecord.IsVisibleToClients;
        if (Providers != otherRecord.Providers) Providers = otherRecord.Providers;
        if (IsInherited != otherRecord.IsInherited) IsInherited = otherRecord.IsInherited;
        if (IsEncrypted != otherRecord.IsEncrypted) IsEncrypted = otherRecord.IsEncrypted;
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
