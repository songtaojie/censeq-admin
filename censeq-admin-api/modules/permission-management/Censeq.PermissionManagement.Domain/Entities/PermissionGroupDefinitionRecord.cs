using System;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Censeq.PermissionManagement.Entities;

public class PermissionGroupDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    public PermissionGroupDefinitionRecord()
    {
        this.SetDefaultsForExtraProperties();
    }

    public PermissionGroupDefinitionRecord(Guid id, string name, string? displayName) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), PermissionGroupDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), PermissionGroupDefinitionRecordConsts.MaxDisplayNameLength);
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(PermissionGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name || DisplayName != otherRecord.DisplayName)
            return false;
        return this.HasSameExtraProperties(otherRecord);
    }

    public void Patch(PermissionGroupDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name) Name = otherRecord.Name;
        if (DisplayName != otherRecord.DisplayName) DisplayName = otherRecord.DisplayName;
        if (!this.HasSameExtraProperties(otherRecord))
        {
            ExtraProperties.Clear();
            foreach (var property in otherRecord.ExtraProperties)
                ExtraProperties.Add(property.Key, property.Value);
        }
    }
}
