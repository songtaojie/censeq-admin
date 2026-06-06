using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.AuditLogging.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志应用服务，提供审计日志查询和删除能力。
/// </summary>
[Authorize(AuditLoggingPermissions.AuditLogs)]
public class AuditLogAppService : ApplicationService, IAuditLogAppService
{
    /// <summary>
    /// 审计日志仓储。
    /// </summary>
    protected IAuditLogRepository AuditLogRepository { get; }

    /// <summary>
    /// 初始化 AuditLogAppService 实例。
    /// </summary>
    /// <param name="auditLogRepository">审计日志仓储。</param>
    public AuditLogAppService(IAuditLogRepository auditLogRepository)
    {
        AuditLogRepository = auditLogRepository;
    }

    /// <summary>
    /// 异步获取审计日志列表。
    /// </summary>
    /// <param name="input">查询输入。</param>
    /// <returns>审计日志列表。</returns>
    public virtual async Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input)
    {
        var count = await AuditLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            httpMethod: input.HttpMethod,
            url: input.Url,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            clientIpAddress: input.ClientIpAddress,
            hasException: input.HasException,
            minExecutionDuration: input.MinExecutionDuration,
            maxExecutionDuration: input.MaxExecutionDuration
        );

        var list = await AuditLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            httpMethod: input.HttpMethod,
            url: input.Url,
            userName: input.UserName,
            applicationName: input.ApplicationName,
            clientIpAddress: input.ClientIpAddress,
            hasException: input.HasException,
            minExecutionDuration: input.MinExecutionDuration,
            maxExecutionDuration: input.MaxExecutionDuration
        );

        var dtos = list.Select(x => MapToAuditLogDto(x)).ToList();

        return new PagedResultDto<AuditLogDto>(count, dtos);
    }

    /// <summary>
    /// 异步获取审计日志详情。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>审计日志详情。</returns>
    public virtual async Task<AuditLogDto> GetAsync(Guid id)
    {
        var auditLog = await AuditLogRepository.GetAsync(id);
        return MapToAuditLogDto(auditLog, includeDetails: true);
    }

    /// <summary>
    /// 异步删除审计日志。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>表示异步操作的任务。</returns>
    [Authorize(AuditLoggingPermissions.AuditLogsDelete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await AuditLogRepository.DeleteAsync(id);
    }

    /// <summary>
    /// 将审计日志实体映射为 DTO。
    /// </summary>
    /// <param name="auditLog">审计日志。</param>
    /// <param name="includeDetails">是否包含详情。</param>
    /// <returns>审计日志 DTO。</returns>
    protected virtual AuditLogDto MapToAuditLogDto(AuditLog auditLog, bool includeDetails = false)
    {
        var dto = new AuditLogDto
        {
            Id = auditLog.Id,
            ApplicationName = auditLog.ApplicationName,
            UserId = auditLog.UserId,
            UserName = auditLog.UserName,
            TenantId = auditLog.TenantId,
            TenantName = auditLog.TenantName,
            ExecutionTime = auditLog.ExecutionTime,
            ExecutionDuration = auditLog.ExecutionDuration,
            ClientIpAddress = auditLog.ClientIpAddress,
            ClientName = auditLog.ClientName,
            ClientId = auditLog.ClientId,
            CorrelationId = auditLog.CorrelationId,
            BrowserInfo = auditLog.BrowserInfo,
            HttpMethod = auditLog.HttpMethod,
            Url = auditLog.Url,
            Exceptions = auditLog.Exceptions,
            Comments = auditLog.Comments,
            HttpStatusCode = auditLog.HttpStatusCode,
            HasException = !string.IsNullOrEmpty(auditLog.Exceptions)
        };

        if (includeDetails)
        {
            dto.EntityChanges = auditLog.EntityChanges.Select(ec => new EntityChangeDto
            {
                Id = ec.Id,
                AuditLogId = ec.AuditLogId,
                ChangeTime = ec.ChangeTime,
                ChangeType = (byte)ec.ChangeType,
                EntityId = ec.EntityId,
                EntityTypeFullName = ec.EntityTypeFullName,
                PropertyChanges = ec.PropertyChanges.Select(pc => new EntityPropertyChangeDto
                {
                    Id = pc.Id,
                    EntityChangeId = pc.EntityChangeId,
                    PropertyName = pc.PropertyName,
                    PropertyTypeFullName = pc.PropertyTypeFullName,
                    OriginalValue = pc.OriginalValue,
                    NewValue = pc.NewValue
                }).ToList()
            }).ToList();

            dto.Actions = auditLog.Actions.Select(a => new AuditLogActionDto
            {
                Id = a.Id,
                AuditLogId = a.AuditLogId,
                ServiceName = a.ServiceName,
                MethodName = a.MethodName,
                Parameters = a.Parameters,
                ExecutionTime = a.ExecutionTime,
                ExecutionDuration = a.ExecutionDuration
            }).ToList();
        }

        return dto;
    }
}
