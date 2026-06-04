using System;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Censeq.PermissionManagement.Entities;

/// <summary>
/// 权限组持久化记录。
/// </summary>
public class PermissionGroup : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    /// <summary>
    /// 权限组名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 显示名称。
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>系统原始多语言 key，格式为 L:ResourceName,Key，只读，由系统同步写入</summary>
    public string? LocalizationKey { get; set; }
    /// <summary>
    /// 扩展属性。
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = [];

    /// <summary>
    /// 初始化 PermissionGroup 实例。
    /// </summary>
    public PermissionGroup()
    {
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// 初始化 PermissionGroup 实例。
    /// </summary>
    /// <param name="id">实体标识。</param>
    /// <param name="name">权限名称。</param>
    /// <param name="displayName">显示名称。</param>
    /// <param name="localizationKey">本地化资源键。</param>
    public PermissionGroup(Guid id, string name, string? displayName, string? localizationKey = null) : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), PermissionGroupConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), PermissionGroupConsts.MaxDisplayNameLength);
        LocalizationKey = localizationKey;
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// 判断当前记录与指定记录的数据是否一致。
    /// </summary>
    /// <param name="otherRecord">用于比较或更新的记录。</param>
    /// <returns>数据一致时返回 true，否则返回 false。</returns>
    public bool HasSameData(PermissionGroup otherRecord)
    {
        // DisplayName 是用户可编辑字段，不参与系统同步的变更检测
        if (Name != otherRecord.Name || LocalizationKey != otherRecord.LocalizationKey)
            return false;
        return this.HasSameExtraProperties(otherRecord);
    }

    /// <summary>
    /// 使用指定记录修补当前记录。
    /// </summary>
    /// <param name="otherRecord">用于比较或更新的记录。</param>
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