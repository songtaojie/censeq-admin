using System;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement.Entities;

public class PermissionDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public required string GroupName { get; set; }
    public required string Name { get; set; }
    public string? ParentName { get; set; }
    public required string DisplayName { get; set; }
    /// <summary>系统原始多语言 key，格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }
    public bool IsEnabled { get; set; }
    public MultiTenancySides MultiTenancySide { get; set; }
    public string? Providers { get; set; }
    public string? StateCheckers { get; set; }
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    public PermissionDefinitionRecord()
    {
        GroupName = string.Empty;
        Name = string.Empty;
        DisplayName = string.Empty;
        ExtraProperties = [];
        this.SetDefaultsForExtraProperties();
    }

    public PermissionDefinitionRecord(Guid id) : base(id)
    {
        ExtraProperties = [];
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(PermissionDefinitionRecord? otherRecord)
    {
        if (otherRecord == null) return false;
        // DisplayName 是用户可编辑字段，不参与系统同步的变更检测
        if (Name != otherRecord.Name || GroupName != otherRecord.GroupName || ParentName != otherRecord.ParentName ||
            LocalizationKey != otherRecord.LocalizationKey ||
            IsEnabled != otherRecord.IsEnabled ||
            MultiTenancySide != otherRecord.MultiTenancySide || Providers != otherRecord.Providers ||
            StateCheckers != otherRecord.StateCheckers)
            return false;
        return this.HasSameExtraProperties(otherRecord);
    }

    public void Patch(PermissionDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name) Name = otherRecord.Name;
        if (GroupName != otherRecord.GroupName) GroupName = otherRecord.GroupName;
        if (ParentName != otherRecord.ParentName) ParentName = otherRecord.ParentName;
        // DisplayName 是用户可编辑字段，系统同步时不覆盖
        // 只在新增时（由 StaticPermissionSaver 直接 Insert）初始化
        if (LocalizationKey != otherRecord.LocalizationKey) LocalizationKey = otherRecord.LocalizationKey;
        if (IsEnabled != otherRecord.IsEnabled) IsEnabled = otherRecord.IsEnabled;
        if (MultiTenancySide != otherRecord.MultiTenancySide) MultiTenancySide = otherRecord.MultiTenancySide;
        if (Providers != otherRecord.Providers) Providers = otherRecord.Providers;
        if (StateCheckers != otherRecord.StateCheckers) StateCheckers = otherRecord.StateCheckers;
        if (!this.HasSameExtraProperties(otherRecord))
        {
            ExtraProperties.Clear();
            foreach (var property in otherRecord.ExtraProperties)
                ExtraProperties.Add(property.Key, property.Value);
        }
    }
}
