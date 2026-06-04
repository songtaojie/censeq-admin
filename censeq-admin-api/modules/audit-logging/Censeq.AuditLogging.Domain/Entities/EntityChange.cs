using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.AuditLogging.Entities;

/// <summary>
/// 实体变更记录实体。
/// </summary>
[DisableAuditing]
public class EntityChange : Entity<Guid>, IMultiTenant, IHasExtraProperties
{
    /// <summary>
    /// 审计日志标识。
    /// </summary>
    public virtual Guid AuditLogId { get; protected set; }

    /// <summary>
    /// 租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 变更时间。
    /// </summary>
    public virtual DateTime ChangeTime { get; protected set; }

    /// <summary>
    /// 变更类型。
    /// </summary>
    public virtual EntityChangeType ChangeType { get; protected set; }

    /// <summary>
    /// 实体租户标识。
    /// </summary>
    public virtual Guid? EntityTenantId { get; protected set; }

    /// <summary>
    /// 实体标识。
    /// </summary>
    public virtual string? EntityId { get; protected set; }

    /// <summary>
    /// 实体类型完整名称。
    /// </summary>
    public virtual string? EntityTypeFullName { get; protected set; }

    /// <summary>
    /// 属性变更集合。
    /// </summary>
    public virtual ICollection<EntityPropertyChange> PropertyChanges { get; protected set; }

    /// <summary>
    /// 扩展属性。
    /// </summary>
    public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; }

    /// <summary>
    /// 初始化 EntityChange 实例。
    /// </summary>
    protected EntityChange()
    {
        ExtraProperties = [];
        PropertyChanges = [];
    }

    /// <summary>
    /// 初始化 EntityChange 实例。
    /// </summary>
    /// <param name="guidGenerator">guidGenerator。</param>
    /// <param name="auditLogId">auditLogId。</param>
    /// <param name="entityChangeInfo">entityChangeInfo。</param>
    /// <param name="tenantId">租户标识。</param>
    public EntityChange(
        IGuidGenerator guidGenerator,
        Guid auditLogId,
        EntityChangeInfo entityChangeInfo,
        Guid? tenantId = null)
    {
        Id = guidGenerator.Create();
        AuditLogId = auditLogId;
        TenantId = tenantId;
        ChangeTime = entityChangeInfo.ChangeTime;
        ChangeType = entityChangeInfo.ChangeType;
        EntityTenantId = entityChangeInfo.EntityTenantId;
        EntityId = entityChangeInfo.EntityId.Truncate(EntityChangeConsts.MaxEntityTypeFullNameLength);
        EntityTypeFullName = entityChangeInfo.EntityTypeFullName.TruncateFromBeginning(EntityChangeConsts.MaxEntityTypeFullNameLength);

        PropertyChanges = entityChangeInfo
                              .PropertyChanges?
                              .Select(p => new EntityPropertyChange(guidGenerator, Id, p, tenantId))
                              .ToList()
                          ?? [];

        ExtraProperties = [];
        if (entityChangeInfo.ExtraProperties != null)
        {
            foreach (var pair in entityChangeInfo.ExtraProperties)
            {
                ExtraProperties.Add(pair.Key, pair.Value);
            }
        }
    }
}
