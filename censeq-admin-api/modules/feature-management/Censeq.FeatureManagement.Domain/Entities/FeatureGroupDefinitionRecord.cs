using System;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Censeq.FeatureManagement.Entities;

public class FeatureGroupDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    /// <summary>系统原始多语言 key，格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public FeatureGroupDefinitionRecord()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public FeatureGroupDefinitionRecord(
        Guid id,
        string name,
        string displayName,
        string? localizationKey = null)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), FeatureGroupDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), FeatureGroupDefinitionRecordConsts.MaxDisplayNameLength);
        LocalizationKey = localizationKey;

        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(FeatureGroupDefinitionRecord otherRecord)
    {
        // DisplayName 是用户可编辑字段，不参与系统同步的变更检测
        if (Name != otherRecord.Name || LocalizationKey != otherRecord.LocalizationKey)
        {
            return false;
        }

        if (!this.HasSameExtraProperties(otherRecord))
        {
            return false;
        }

        return true;
    }

    public void Patch(FeatureGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            Name = otherRecord.Name;
        }

        // DisplayName 是用户可编辑字段，系统同步时不覆盖
        if (LocalizationKey != otherRecord.LocalizationKey)
        {
            LocalizationKey = otherRecord.LocalizationKey;
        }

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
