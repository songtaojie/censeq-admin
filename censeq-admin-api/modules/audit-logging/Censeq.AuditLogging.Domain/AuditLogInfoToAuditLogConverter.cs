using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.AuditLogging.Entities;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Http;
using Volo.Abp.Json;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志信息转换器，用于创建审计日志实体。
/// </summary>
public class AuditLogInfoToAuditLogConverter : IAuditLogInfoToAuditLogConverter, ITransientDependency
{
    /// <summary>
    /// GUID 生成器。
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// 异常错误信息转换器。
    /// </summary>
    protected IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter { get; }
    /// <summary>
    /// JSON 序列化器。
    /// </summary>
    protected IJsonSerializer JsonSerializer { get; }
    /// <summary>
    /// 异常处理配置项。
    /// </summary>
    protected AbpExceptionHandlingOptions ExceptionHandlingOptions { get; }

    /// <summary>
    /// 初始化 AuditLogInfoToAuditLogConverter 实例。
    /// </summary>
    /// <param name="guidGenerator">guidGenerator。</param>
    /// <param name="exceptionToErrorInfoConverter">exceptionToErrorInfoConverter。</param>
    /// <param name="jsonSerializer">jsonSerializer。</param>
    /// <param name="exceptionHandlingOptions">exceptionHandlingOptions。</param>
    public AuditLogInfoToAuditLogConverter(IGuidGenerator guidGenerator, IExceptionToErrorInfoConverter exceptionToErrorInfoConverter, IJsonSerializer jsonSerializer, IOptions<AbpExceptionHandlingOptions> exceptionHandlingOptions)
    {
        GuidGenerator = guidGenerator;
        ExceptionToErrorInfoConverter = exceptionToErrorInfoConverter;
        JsonSerializer = jsonSerializer;
        ExceptionHandlingOptions = exceptionHandlingOptions.Value;
    }

    /// <summary>
    /// 异步转换审计日志信息。
    /// </summary>
    /// <param name="auditLogInfo">审计日志信息。</param>
    /// <returns>审计日志实体。</returns>
    public virtual Task<AuditLog> ConvertAsync(AuditLogInfo auditLogInfo)
    {
        var auditLogId = GuidGenerator.Create();

        var extraProperties = new ExtraPropertyDictionary();
        if (auditLogInfo.ExtraProperties != null)
        {
            foreach (var pair in auditLogInfo.ExtraProperties)
            {
                extraProperties.Add(pair.Key, pair.Value);
            }
        }

        var entityChanges = auditLogInfo
                                .EntityChanges?
                                .Select(entityChangeInfo => new EntityChange(GuidGenerator, auditLogId, entityChangeInfo, tenantId: auditLogInfo.TenantId))
                                .ToList()
                            ?? new List<EntityChange>();

        var actions = auditLogInfo
                          .Actions?
                          .Select(auditLogActionInfo => new AuditLogAction(GuidGenerator.Create(), auditLogId, auditLogActionInfo, tenantId: auditLogInfo.TenantId))
                          .ToList()
                      ?? new List<AuditLogAction>();

        var remoteServiceErrorInfos = auditLogInfo.Exceptions?.Select(exception => ExceptionToErrorInfoConverter.Convert(exception, options =>
                                          {
                                              options.SendExceptionsDetailsToClients = ExceptionHandlingOptions.SendExceptionsDetailsToClients;
                                              options.SendStackTraceToClients = ExceptionHandlingOptions.SendStackTraceToClients;
                                          }))
                                      ?? new List<RemoteServiceErrorInfo>();

        var exceptions = remoteServiceErrorInfos.Any()
            ? JsonSerializer.Serialize(remoteServiceErrorInfos, indented: true)
            : null;

        var comments = auditLogInfo
            .Comments?
            .JoinAsString(Environment.NewLine);

        var auditLog = new AuditLog(
            auditLogId,
            auditLogInfo.ApplicationName,
            auditLogInfo.TenantId,
            auditLogInfo.TenantName,
            auditLogInfo.UserId,
            auditLogInfo.UserName,
            auditLogInfo.ExecutionTime,
            auditLogInfo.ExecutionDuration,
            auditLogInfo.ClientIpAddress,
            auditLogInfo.ClientName,
            auditLogInfo.ClientId,
            auditLogInfo.CorrelationId,
            auditLogInfo.BrowserInfo,
            auditLogInfo.HttpMethod,
            auditLogInfo.Url,
            auditLogInfo.HttpStatusCode,
            auditLogInfo.ImpersonatorUserId,
            auditLogInfo.ImpersonatorUserName,
            auditLogInfo.ImpersonatorTenantId,
            auditLogInfo.ImpersonatorTenantName,
            extraProperties,
            entityChanges,
            actions,
            exceptions,
            comments
        );

        return Task.FromResult(auditLog);
    }
}
