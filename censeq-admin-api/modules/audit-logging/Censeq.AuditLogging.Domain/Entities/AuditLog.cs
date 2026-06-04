using System;
using System.Collections.Generic;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.AuditLogging.Entities;

/// <summary>
/// 审计日志实体。
/// </summary>
[DisableAuditing]
public class AuditLog : AggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 应用程序名称。
    /// </summary>
    public virtual string? ApplicationName { get; set; }

    /// <summary>
    /// 用户标识。
    /// </summary>
    public virtual Guid? UserId { get; protected set; }

    /// <summary>
    /// 用户名。
    /// </summary>
    public virtual string? UserName { get; protected set; }

    /// <summary>
    /// 租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 租户名称。
    /// </summary>
    public virtual string? TenantName { get; protected set; }

    /// <summary>
    /// 模拟用户标识。
    /// </summary>
    public virtual Guid? ImpersonatorUserId { get; protected set; }

    /// <summary>
    /// 模拟用户名。
    /// </summary>
    public virtual string? ImpersonatorUserName { get; protected set; }

    /// <summary>
    /// 模拟租户标识。
    /// </summary>
    public virtual Guid? ImpersonatorTenantId { get; protected set; }

    /// <summary>
    /// 模拟租户名称。
    /// </summary>
    public virtual string? ImpersonatorTenantName { get; protected set; }

    /// <summary>
    /// 执行时间。
    /// </summary>
    public virtual DateTime ExecutionTime { get; protected set; }

    /// <summary>
    /// 执行耗时。
    /// </summary>
    public virtual int ExecutionDuration { get; protected set; }

    /// <summary>
    /// 客户端 IP 地址。
    /// </summary>
    public virtual string? ClientIpAddress { get; protected set; }

    /// <summary>
    /// 客户端名称。
    /// </summary>
    public virtual string? ClientName { get; protected set; }

    /// <summary>
    /// 客户端标识。
    /// </summary>
    public virtual string? ClientId { get; set; }

    /// <summary>
    /// 关联标识。
    /// </summary>
    public virtual string? CorrelationId { get; set; }

    /// <summary>
    /// 浏览器信息。
    /// </summary>
    public virtual string? BrowserInfo { get; protected set; }

    /// <summary>
    /// HTTP 方法。
    /// </summary>
    public virtual string? HttpMethod { get; protected set; }

    /// <summary>
    /// 请求地址。
    /// </summary>
    public virtual string? Url { get; protected set; }

    /// <summary>
    /// 异常信息。
    /// </summary>
    public virtual string? Exceptions { get; protected set; }

    /// <summary>
    /// 备注。
    /// </summary>
    public virtual string? Comments { get; protected set; }

    /// <summary>
    /// HTTP 状态码。
    /// </summary>
    public virtual int? HttpStatusCode { get; set; }

    /// <summary>
    /// 实体变更集合。
    /// </summary>
    public virtual ICollection<EntityChange> EntityChanges { get; protected set; }

    /// <summary>
    /// 审计日志操作集合。
    /// </summary>
    public virtual ICollection<AuditLogAction> Actions { get; protected set; }

    /// <summary>
    /// 初始化 AuditLog 实例。
    /// </summary>
    protected AuditLog()
    {
        EntityChanges = [];
        Actions = [];
    }

    /// <summary>
    /// 初始化 AuditLog 实例。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="applicationName">应用程序名称。</param>
    /// <param name="tenantId">租户标识。</param>
    /// <param name="tenantName">租户名称。</param>
    /// <param name="userId">用户标识。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="executionTime">执行时间。</param>
    /// <param name="executionDuration">执行耗时。</param>
    /// <param name="clientIpAddress">客户端 IP 地址。</param>
    /// <param name="clientName">客户端名称。</param>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="correlationId">关联标识。</param>
    /// <param name="browserInfo">浏览器信息。</param>
    /// <param name="httpMethod">HTTP 方法。</param>
    /// <param name="url">请求地址。</param>
    /// <param name="httpStatusCode">HTTP 状态码。</param>
    /// <param name="impersonatorUserId">模拟用户标识。</param>
    /// <param name="impersonatorUserName">模拟用户名。</param>
    /// <param name="impersonatorTenantId">模拟租户标识。</param>
    /// <param name="impersonatorTenantName">模拟租户名称。</param>
    /// <param name="extraPropertyDictionary">扩展属性字典。</param>
    /// <param name="entityChanges">实体变更列表。</param>
    /// <param name="actions">审计日志操作列表。</param>
    /// <param name="exceptions">异常信息。</param>
    /// <param name="comments">备注。</param>
    public AuditLog(
        Guid id,
        string? applicationName,
        Guid? tenantId,
        string? tenantName,
        Guid? userId,
        string? userName,
        DateTime executionTime,
        int executionDuration,
        string? clientIpAddress,
        string? clientName,
        string? clientId,
        string? correlationId,
        string? browserInfo,
        string? httpMethod,
        string? url,
        int? httpStatusCode,
        Guid? impersonatorUserId,
        string? impersonatorUserName,
        Guid? impersonatorTenantId,
        string? impersonatorTenantName,
        ExtraPropertyDictionary extraPropertyDictionary,
        List<EntityChange> entityChanges,
        List<AuditLogAction> actions,
        string? exceptions,
        string? comments)
        : base(id)
    {
        ApplicationName = applicationName.Truncate(AuditLogConsts.MaxApplicationNameLength);
        TenantId = tenantId;
        TenantName = tenantName.Truncate(AuditLogConsts.MaxTenantNameLength);
        UserId = userId;
        UserName = userName.Truncate(AuditLogConsts.MaxUserNameLength);
        ExecutionTime = executionTime;
        ExecutionDuration = executionDuration;
        ClientIpAddress = clientIpAddress.Truncate(AuditLogConsts.MaxClientIpAddressLength);
        ClientName = clientName.Truncate(AuditLogConsts.MaxClientNameLength);
        ClientId = clientId.Truncate(AuditLogConsts.MaxClientIdLength);
        CorrelationId = correlationId.Truncate(AuditLogConsts.MaxCorrelationIdLength);
        BrowserInfo = browserInfo.Truncate(AuditLogConsts.MaxBrowserInfoLength);
        HttpMethod = httpMethod.Truncate(AuditLogConsts.MaxHttpMethodLength);
        Url = url.Truncate(AuditLogConsts.MaxUrlLength);
        HttpStatusCode = httpStatusCode;
        ImpersonatorUserId = impersonatorUserId;
        ImpersonatorUserName = impersonatorUserName.Truncate(AuditLogConsts.MaxUserNameLength);
        ImpersonatorTenantId = impersonatorTenantId;
        ImpersonatorTenantName = impersonatorTenantName.Truncate(AuditLogConsts.MaxTenantNameLength);
        ExtraProperties = extraPropertyDictionary;
        EntityChanges = entityChanges;
        Actions = actions;
        Exceptions = exceptions;
        Comments = comments.Truncate(AuditLogConsts.MaxCommentsLength);
    }
}
