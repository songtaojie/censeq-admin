using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.AuditLogging.Entities;

/// <summary>
/// 实体属性变更记录实体。
/// </summary>
[DisableAuditing]
public class EntityPropertyChange : Entity<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 实体变更标识。
    /// </summary>
    public virtual Guid EntityChangeId { get; protected set; }

    /// <summary>
    /// 新值。
    /// </summary>
    public virtual string? NewValue { get; protected set; }

    /// <summary>
    /// 原始值。
    /// </summary>
    public virtual string? OriginalValue { get; protected set; }

    /// <summary>
    /// 属性名称。
    /// </summary>
    public virtual string? PropertyName { get; protected set; }

    /// <summary>
    /// 属性类型完整名称。
    /// </summary>
    public virtual string? PropertyTypeFullName { get; protected set; }

    /// <summary>
    /// 初始化 EntityPropertyChange 实例。
    /// </summary>
    protected EntityPropertyChange()
    {

    }

    /// <summary>
    /// 初始化 EntityPropertyChange 实例。
    /// </summary>
    /// <param name="guidGenerator">guidGenerator。</param>
    /// <param name="entityChangeId">实体变更标识。</param>
    /// <param name="entityChangeInfo">entityChangeInfo。</param>
    /// <param name="tenantId">租户标识。</param>
    public EntityPropertyChange(
        IGuidGenerator guidGenerator,
        Guid entityChangeId,
        EntityPropertyChangeInfo entityChangeInfo,
        Guid? tenantId = null)
    {
        Id = guidGenerator.Create();
        TenantId = tenantId;
        EntityChangeId = entityChangeId;
        NewValue = entityChangeInfo.NewValue.Truncate(EntityPropertyChangeConsts.MaxNewValueLength);
        OriginalValue = entityChangeInfo.OriginalValue.Truncate(EntityPropertyChangeConsts.MaxOriginalValueLength);
        PropertyName = entityChangeInfo.PropertyName.TruncateFromBeginning(EntityPropertyChangeConsts.MaxPropertyNameLength);
        PropertyTypeFullName = entityChangeInfo.PropertyTypeFullName.TruncateFromBeginning(EntityPropertyChangeConsts.MaxPropertyTypeFullNameLength);
    }
}
