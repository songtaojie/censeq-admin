using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 身份用户委托
/// </summary>
public class IdentityUserDelegation : BasicAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid SourceUserId { get; protected set; }

    public virtual Guid TargetUserId { get; protected set; }

    public virtual DateTime StartTime { get; protected set; }

    public virtual DateTime EndTime { get; protected set; }

    /// <summary>
    /// 初始化 <see cref="IdentityUserDelegation"/> 类的新实例。
    /// </summary>
    protected IdentityUserDelegation()
    {
    }

    public IdentityUserDelegation(
        Guid id,
        Guid sourceUserId,
        Guid targetUserId,
        DateTime startTime,
        DateTime endTime,
        Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        SourceUserId = sourceUserId;
        TargetUserId = targetUserId;
        StartTime = startTime;
        EndTime = endTime;
    }
}
