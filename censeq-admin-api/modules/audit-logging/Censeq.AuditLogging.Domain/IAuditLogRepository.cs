using Censeq.AuditLogging.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志仓储接口。
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog, Guid>
{
    /// <summary>
    /// 异步获取审计日志列表。
    /// </summary>
    /// <param name="sorting">排序条件。</param>
    /// <param name="maxResultCount">最大返回数量。</param>
    /// <param name="skipCount">跳过数量。</param>
    /// <param name="startTime">开始时间。</param>
    /// <param name="endTime">结束时间。</param>
    /// <param name="httpMethod">HTTP 方法。</param>
    /// <param name="url">请求地址。</param>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="userId">用户标识。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="applicationName">应用程序名称。</param>
    /// <param name="clientIpAddress">客户端 IP 地址。</param>
    /// <param name="correlationId">关联标识。</param>
    /// <param name="maxExecutionDuration">最大执行耗时。</param>
    /// <param name="minExecutionDuration">最小执行耗时。</param>
    /// <param name="hasException">是否存在异常。</param>
    /// <param name="httpStatusCode">HTTP 状态码。</param>
    /// <param name="includeDetails">是否包含详情。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>审计日志列表。</returns>
    Task<List<AuditLog>> GetListAsync(
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? httpMethod = null,
        string? url = null,
        string? clientId = null,
        Guid? userId = null,
        string? userName = null,
        string? applicationName = null,
        string? clientIpAddress = null,
        string? correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        HttpStatusCode? httpStatusCode = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取审计日志数量。
    /// </summary>
    /// <param name="startTime">开始时间。</param>
    /// <param name="endTime">结束时间。</param>
    /// <param name="httpMethod">HTTP 方法。</param>
    /// <param name="url">请求地址。</param>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="userId">用户标识。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="applicationName">应用程序名称。</param>
    /// <param name="clientIpAddress">客户端 IP 地址。</param>
    /// <param name="correlationId">关联标识。</param>
    /// <param name="maxExecutionDuration">最大执行耗时。</param>
    /// <param name="minExecutionDuration">最小执行耗时。</param>
    /// <param name="hasException">是否存在异常。</param>
    /// <param name="httpStatusCode">HTTP 状态码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>审计日志数量。</returns>
    Task<long> GetCountAsync(
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? httpMethod = null,
        string? url = null,
        string? clientId = null,
        Guid? userId = null,
        string? userName = null,
        string? applicationName = null,
        string? clientIpAddress = null,
        string? correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        HttpStatusCode? httpStatusCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取每日平均执行耗时。
    /// </summary>
    /// <param name="startDate">startDate。</param>
    /// <param name="endDate">endDate。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>每日平均执行耗时。</returns>
    Task<Dictionary<DateTime, double>> GetAverageExecutionDurationPerDayAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体变更记录。
    /// </summary>
    /// <param name="entityChangeId">实体变更标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实体变更记录。</returns>
    Task<EntityChange> GetEntityChange(Guid entityChangeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取实体变更列表。
    /// </summary>
    /// <param name="sorting">排序条件。</param>
    /// <param name="maxResultCount">最大返回数量。</param>
    /// <param name="skipCount">跳过数量。</param>
    /// <param name="auditLogId">auditLogId。</param>
    /// <param name="startTime">开始时间。</param>
    /// <param name="endTime">结束时间。</param>
    /// <param name="changeType">变更类型。</param>
    /// <param name="entityId">实体标识。</param>
    /// <param name="entityTypeFullName">实体类型完整名称。</param>
    /// <param name="includeDetails">是否包含详情。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实体变更列表。</returns>
    Task<List<EntityChange>> GetEntityChangeListAsync(
        string? sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string? entityId = null,
        string? entityTypeFullName = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取实体变更数量。
    /// </summary>
    /// <param name="auditLogId">auditLogId。</param>
    /// <param name="startTime">开始时间。</param>
    /// <param name="endTime">结束时间。</param>
    /// <param name="changeType">变更类型。</param>
    /// <param name="entityId">实体标识。</param>
    /// <param name="entityTypeFullName">实体类型完整名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实体变更数量。</returns>
    Task<long> GetEntityChangeCountAsync(
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string? entityId = null,
        string? entityTypeFullName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取带用户名的实体变更详情。
    /// </summary>
    /// <param name="entityChangeId">实体变更标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带用户名的实体变更详情。</returns>
    Task<EntityChangeWithUsername> GetEntityChangeWithUsernameAsync(Guid entityChangeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取带用户名的实体变更列表。
    /// </summary>
    /// <param name="entityId">实体标识。</param>
    /// <param name="entityTypeFullName">实体类型完整名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带用户名的实体变更列表。</returns>
    Task<List<EntityChangeWithUsername>> GetEntityChangesWithUsernameAsync(string entityId, string entityTypeFullName, CancellationToken cancellationToken = default);
}
