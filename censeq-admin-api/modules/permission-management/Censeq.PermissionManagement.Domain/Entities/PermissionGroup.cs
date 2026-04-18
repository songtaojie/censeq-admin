using System;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Censeq.PermissionManagement.Entities;

public class PermissionGroup : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    public PermissionGroup()
    {
        this.SetDefaultsForExtraProperties();
    }

    public PermissionGroup(Guid id, string name, string? displayName) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), PermissionGroupConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), PermissionGroupConsts.MaxDisplayNameLength);
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(PermissionGroup otherRecord)
    {
        if (Name != otherRecord.Name || DisplayName != otherRecord.DisplayName)
            return false;
        return this.HasSameExtraProperties(otherRecord);
    }

    public void Patch(PermissionGroup otherRecord)
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