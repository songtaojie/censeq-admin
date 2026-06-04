using System;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.AuditLogging.Entities;

/// <summary>
/// 审计日志操作实体。
/// </summary>
[DisableAuditing]
public class AuditLogAction : Entity<Guid>, IMultiTenant, IHasExtraProperties
{
    /// <summary>
    /// 租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 审计日志标识。
    /// </summary>
    public virtual Guid AuditLogId { get; protected set; }

    /// <summary>
    /// 服务名称。
    /// </summary>
    public virtual string? ServiceName { get; protected set; }

    /// <summary>
    /// 方法名称。
    /// </summary>
    public virtual string? MethodName { get; protected set; }

    /// <summary>
    /// 方法参数。
    /// </summary>
    public virtual string? Parameters { get; protected set; }

    /// <summary>
    /// 执行时间。
    /// </summary>
    public virtual DateTime ExecutionTime { get; protected set; }

    /// <summary>
    /// 执行耗时。
    /// </summary>
    public virtual int ExecutionDuration { get; protected set; }

    /// <summary>
    /// 扩展属性。
    /// </summary>
    public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; }

    /// <summary>
    /// 初始化 AuditLogAction 实例。
    /// </summary>
    protected AuditLogAction()
    {
        ExtraProperties = [];
    }

    /// <summary>
    /// 初始化 AuditLogAction 实例。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="auditLogId">auditLogId。</param>
    /// <param name="actionInfo">actionInfo。</param>
    /// <param name="tenantId">租户标识。</param>
    public AuditLogAction(Guid id, Guid auditLogId, AuditLogActionInfo actionInfo, Guid? tenantId = null)
    {

        Id = id;
        TenantId = tenantId;
        AuditLogId = auditLogId;
        ExecutionTime = actionInfo.ExecutionTime;
        ExecutionDuration = actionInfo.ExecutionDuration;
        ExtraProperties = new ExtraPropertyDictionary(actionInfo.ExtraProperties);
        ServiceName = actionInfo.ServiceName.TruncateFromBeginning(AuditLogActionConsts.MaxServiceNameLength);
        MethodName = actionInfo.MethodName.TruncateFromBeginning(AuditLogActionConsts.MaxMethodNameLength);
        Parameters = actionInfo.Parameters.Length > AuditLogActionConsts.MaxParametersLength ? "" : actionInfo.Parameters;
    }
}
