using System;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement;

public class PermissionDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public required string GroupName { get; set; }
    public required string Name { get; set; }
    public string? ParentName { get; set; }
    public required string DisplayName { get; set; }
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
        if (Name != otherRecord.Name || GroupName != otherRecord.GroupName || ParentName != otherRecord.ParentName ||
            DisplayName != otherRecord.DisplayName || IsEnabled != otherRecord.IsEnabled ||
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
        if (DisplayName != otherRecord.DisplayName) DisplayName = otherRecord.DisplayName;
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
