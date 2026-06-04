using System;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement.Entities;

/// <summary>
/// 权限定义持久化记录。
/// </summary>
public class PermissionDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    /// <summary>
    /// 权限组名称。
    /// </summary>
    public required string GroupName { get; set; }
    /// <summary>
    /// 权限名称。
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// 父级权限名称。
    /// </summary>
    public string? ParentName { get; set; }
    /// <summary>
    /// 显示名称。
    /// </summary>
    public required string DisplayName { get; set; }
    /// <summary>系统原始多语言 key，格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; }
    /// <summary>
    /// 权限适用的多租户侧。
    /// </summary>
    public MultiTenancySides MultiTenancySide { get; set; }
    /// <summary>
    /// 允许使用该权限的提供者列表。
    /// </summary>
    public string? Providers { get; set; }
    /// <summary>
    /// 权限状态检查器列表。
    /// </summary>
    public string? StateCheckers { get; set; }
    /// <summary>
    /// 扩展属性。
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    /// <summary>
    /// 初始化 PermissionDefinitionRecord 实例。
    /// </summary>
    public PermissionDefinitionRecord()
    {
        GroupName = string.Empty;
        Name = string.Empty;
        DisplayName = string.Empty;
        ExtraProperties = [];
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// 初始化 PermissionDefinitionRecord 实例。
    /// </summary>
    /// <param name="id">实体标识。</param>
    public PermissionDefinitionRecord(Guid id) : base(id)
    {
        ExtraProperties = [];
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// 判断当前记录与指定记录的数据是否一致。
    /// </summary>
    /// <param name="otherRecord">用于比较或更新的记录。</param>
    /// <returns>数据一致时返回 true，否则返回 false。</returns>
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

    /// <summary>
    /// 使用指定记录修补当前记录。
    /// </summary>
    /// <param name="otherRecord">用于比较或更新的记录。</param>
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
