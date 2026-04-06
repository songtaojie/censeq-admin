using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.AuditLogging.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.AuditLogging;

[Authorize]
public class AuditLogAppService : ApplicationService, IAuditLogAppService
{
    protected IAuditLogRepository AuditLogRepository { get; }

    public AuditLogAppService(IAuditLogRepository auditLogRepository)
    {
        AuditLogRepository = auditLogRepository;
    }

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

    public virtual async Task<AuditLogDto> GetAsync(Guid id)
    {
        var auditLog = await AuditLogRepository.GetAsync(id);
        return MapToAuditLogDto(auditLog, includeDetails: true);
    }

    [Authorize]
    public virtual async Task DeleteAsync(Guid id)
    {
        await AuditLogRepository.DeleteAsync(id);
    }

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
