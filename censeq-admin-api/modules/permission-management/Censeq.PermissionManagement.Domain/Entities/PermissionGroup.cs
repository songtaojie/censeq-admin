using System;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Censeq.PermissionManagement.Entities;

public class PermissionGroup : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>系统原始多语言 key，格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    public PermissionGroup()
    {
        this.SetDefaultsForExtraProperties();
    }

    public PermissionGroup(Guid id, string name, string? displayName, string? localizationKey = null) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), PermissionGroupConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), PermissionGroupConsts.MaxDisplayNameLength);
        LocalizationKey = localizationKey;
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(PermissionGroup otherRecord)
    {
        // DisplayName 是用户可编辑字段，不参与系统同步的变更检测
        if (Name != otherRecord.Name || LocalizationKey != otherRecord.LocalizationKey)
            return false;
        return this.HasSameExtraProperties(otherRecord);
    }

    public void Patch(PermissionGroup otherRecord)
    {
        if (Name != otherRecord.Name) Name = otherRecord.Name;
        // DisplayName 是用户可编辑字段，系统同步时不覆盖
        // 只在新增时（由 StaticPermissionSaver 直接 Insert）初始化
        if (LocalizationKey != otherRecord.LocalizationKey) LocalizationKey = otherRecord.LocalizationKey;
        if (!this.HasSameExtraProperties(otherRecord))
        {
            ExtraProperties.Clear();
            foreach (var property in otherRecord.ExtraProperties)
                ExtraProperties.Add(property.Key, property.Value);
        }
    }
}