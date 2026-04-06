using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志DTO
/// </summary>
public class AuditLogDto : EntityDto<Guid>
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 租户名称
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public DateTime ExecutionTime { get; set; }

    /// <summary>
    /// 执行时长(毫秒)
    /// </summary>
    public int ExecutionDuration { get; set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// 客户端名称
    /// </summary>
    public string? ClientName { get; set; }

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// 关联ID
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// 浏览器信息
    /// </summary>
    public string? BrowserInfo { get; set; }

    /// <summary>
    /// HTTP方法
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? Exceptions { get; set; }

    /// <summary>
    /// 注释
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// 是否有异常
    /// </summary>
    public bool HasException { get; set; }

    /// <summary>
    /// 实体变更列表
    /// </summary>
    public List<EntityChangeDto> EntityChanges { get; set; } = new();

    /// <summary>
    /// 操作列表
    /// </summary>
    public List<AuditLogActionDto> Actions { get; set; } = new();
}

/// <summary>
/// 实体变更DTO
/// </summary>
public class EntityChangeDto : EntityDto<Guid>
{
    /// <summary>
    /// 审计日志ID
    /// </summary>
    public Guid AuditLogId { get; set; }

    /// <summary>
    /// 变更时间
    /// </summary>
    public DateTime ChangeTime { get; set; }

    /// <summary>
    /// 变更类型(0:创建, 1:更新, 2:删除)
    /// </summary>
    public byte ChangeType { get; set; }

    /// <summary>
    /// 实体ID
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// 实体类型完整名称
    /// </summary>
    public string? EntityTypeFullName { get; set; }

    /// <summary>
    /// 属性变更列表
    /// </summary>
    public List<EntityPropertyChangeDto> PropertyChanges { get; set; } = new();
}

/// <summary>
/// 实体属性变更DTO
/// </summary>
public class EntityPropertyChangeDto : EntityDto<Guid>
{
    /// <summary>
    /// 实体变更ID
    /// </summary>
    public Guid EntityChangeId { get; set; }

    /// <summary>
    /// 属性名称
    /// </summary>
    public string? PropertyName { get; set; }

    /// <summary>
    /// 属性类型完整名称
    /// </summary>
    public string? PropertyTypeFullName { get; set; }

    /// <summary>
    /// 原始值
    /// </summary>
    public string? OriginalValue { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    public string? NewValue { get; set; }
}

/// <summary>
/// 审计日志操作DTO
/// </summary>
public class AuditLogActionDto : EntityDto<Guid>
{
    /// <summary>
    /// 审计日志ID
    /// </summary>
    public Guid AuditLogId { get; set; }

    /// <summary>
    /// 服务名称
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// 方法名称
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// 参数
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// 执行时间
    /// </summary>
    public DateTime ExecutionTime { get; set; }

    /// <summary>
    /// 执行时长(毫秒)
    /// </summary>
    public int ExecutionDuration { get; set; }
}
