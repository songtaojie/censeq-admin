using Censeq.AuditLogging.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.AuditLogging.EntityFrameworkCore;

/// <summary>
/// EF Core 审计日志仓储，提供持久化查询能力。
/// </summary>
public class EfCoreAuditLogRepository : EfCoreRepository<IAuditLoggingDbContext, AuditLog, Guid>, IAuditLogRepository
{
    /// <summary>
    /// 初始化 EfCoreAuditLogRepository 实例。
    /// </summary>
    /// <param name="dbContextProvider">dbContextProvider。</param>
    /// <param name="abpLazyServiceProvider">abpLazyServiceProvider。</param>
    public EfCoreAuditLogRepository(IDbContextProvider<IAuditLoggingDbContext> dbContextProvider, 
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(dbContextProvider)
    {
        LazyServiceProvider = abpLazyServiceProvider;
    }

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
    public virtual async Task<List<AuditLog>> GetListAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(
            startTime,
            endTime,
            httpMethod,
            url,
            clientId,
            userId,
            userName,
            applicationName,
            clientIpAddress,
            correlationId,
            maxExecutionDuration,
            minExecutionDuration,
            hasException,
            httpStatusCode,
            includeDetails
        );

        var auditLogs = await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(AuditLog.ExecutionTime) + " DESC" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));

        return auditLogs;
    }

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
    public virtual async Task<long> GetCountAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(
            startTime,
            endTime,
            httpMethod,
            url,
            clientId,
            userId,
            userName,
            applicationName,
            clientIpAddress,
            correlationId,
            maxExecutionDuration,
            minExecutionDuration,
            hasException,
            httpStatusCode
        );

        var totalCount = await query.LongCountAsync(GetCancellationToken(cancellationToken));

        return totalCount;
    }

    /// <summary>
    /// 获取审计日志查询。
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
    /// <param name="includeDetails">是否包含详情。</param>
    /// <returns>审计日志查询。</returns>
    protected virtual async Task<IQueryable<AuditLog>> GetListQueryAsync(
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
        bool includeDetails = false)
    {
        var nHttpStatusCode = (int?)httpStatusCode;
        return (await GetDbSetAsync()).AsNoTracking()
            .IncludeDetails(includeDetails)
            .WhereIf(startTime.HasValue, auditLog => auditLog.ExecutionTime >= startTime)
            .WhereIf(endTime.HasValue, auditLog => auditLog.ExecutionTime <= endTime)
            .WhereIf(hasException.HasValue && hasException.Value, auditLog => auditLog.Exceptions != null && auditLog.Exceptions != "")
            .WhereIf(hasException.HasValue && !hasException.Value, auditLog => auditLog.Exceptions == null || auditLog.Exceptions == "")
            .WhereIf(!httpMethod.IsNullOrEmpty(), auditLog => auditLog.HttpMethod == httpMethod)
            .WhereIf(!url.IsNullOrEmpty(), auditLog => auditLog.Url != null && auditLog.Url.Contains(url!))
            .WhereIf(!clientId.IsNullOrEmpty(), auditLog => auditLog.ClientId == clientId)
            .WhereIf(userId != null, auditLog => auditLog.UserId == userId)
            .WhereIf(!userName.IsNullOrEmpty(), auditLog => auditLog.UserName == userName)
            .WhereIf(!applicationName.IsNullOrEmpty(), auditLog => auditLog.ApplicationName == applicationName)
            .WhereIf(!clientIpAddress.IsNullOrEmpty(), auditLog => auditLog.ClientIpAddress != null && auditLog.ClientIpAddress == clientIpAddress)
            .WhereIf(!correlationId.IsNullOrEmpty(), auditLog => auditLog.CorrelationId == correlationId)
            .WhereIf(httpStatusCode != null && httpStatusCode > 0, auditLog => auditLog.HttpStatusCode == nHttpStatusCode)
            .WhereIf(maxExecutionDuration != null && maxExecutionDuration.Value > 0, auditLog => auditLog.ExecutionDuration <= maxExecutionDuration)
            .WhereIf(minExecutionDuration != null && minExecutionDuration.Value > 0, auditLog => auditLog.ExecutionDuration >= minExecutionDuration);
    }

    /// <summary>
    /// 异步获取每日平均执行耗时。
    /// </summary>
    /// <param name="startDate">startDate。</param>
    /// <param name="endDate">endDate。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>每日平均执行耗时。</returns>
    public virtual async Task<Dictionary<DateTime, double>> GetAverageExecutionDurationPerDayAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await (await GetDbSetAsync()).AsNoTracking()
            .Where(a => a.ExecutionTime < endDate.AddDays(1) && a.ExecutionTime > startDate)
            .OrderBy(t => t.ExecutionTime)
            .GroupBy(t => new { t.ExecutionTime.Date })
            .Select(g => new { Day = g.Min(t => t.ExecutionTime), avgExecutionTime = g.Average(t => t.ExecutionDuration) })
            .ToListAsync(GetCancellationToken(cancellationToken));

        return result.ToDictionary(element => element.Day.ClearTime(), element => element.avgExecutionTime);
    }

    /// <summary>
    /// 构建包含详情的查询。
    /// </summary>
    /// <returns>包含详情的查询。</returns>
    [Obsolete("Use WithDetailsAsync method.")]
    public override IQueryable<AuditLog> WithDetails()
    {
        return GetQueryable().IncludeDetails();
    }

    /// <summary>
    /// 异步构建包含详情的查询。
    /// </summary>
    /// <returns>包含详情的查询。</returns>
    public async override Task<IQueryable<AuditLog>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }

    /// <summary>
    /// 获取实体变更记录。
    /// </summary>
    /// <param name="entityChangeId">实体变更标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实体变更记录。</returns>
    public virtual async Task<EntityChange> GetEntityChange(
        Guid entityChangeId,
        CancellationToken cancellationToken = default)
    {
        var entityChange = await (await GetDbContextAsync()).Set<EntityChange>()
                                .AsNoTracking()
                                .IncludeDetails()
                                .Where(x => x.Id == entityChangeId)
                                .OrderBy(x => x.Id)
                                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));

        if (entityChange == null)
        {
            throw new EntityNotFoundException(typeof(EntityChange));
        }

        return entityChange;
    }

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
    public virtual async Task<List<EntityChange>> GetEntityChangeListAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = await GetEntityChangeListQueryAsync(auditLogId, startTime, endTime, changeType, entityId, entityTypeFullName, includeDetails);

        return await query.OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(EntityChange.ChangeTime) + " DESC" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

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
    public virtual async Task<long> GetEntityChangeCountAsync(
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string? entityId = null,
        string? entityTypeFullName = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetEntityChangeListQueryAsync(auditLogId, startTime, endTime, changeType, entityId, entityTypeFullName);

        var totalCount = await query.LongCountAsync(GetCancellationToken(cancellationToken));

        return totalCount;
    }

    /// <summary>
    /// 异步获取带用户名的实体变更详情。
    /// </summary>
    /// <param name="entityChangeId">实体变更标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带用户名的实体变更详情。</returns>
    public virtual async Task<EntityChangeWithUsername> GetEntityChangeWithUsernameAsync(
        Guid entityChangeId,
        CancellationToken cancellationToken = default)
    {
        var auditLog = await (await GetDbSetAsync()).AsNoTracking().IncludeDetails()
            .Where(x => x.EntityChanges.Any(y => y.Id == entityChangeId)).FirstAsync(GetCancellationToken(cancellationToken));

        return new EntityChangeWithUsername()
        {
            EntityChange = auditLog.EntityChanges.First(x => x.Id == entityChangeId),
            UserName = auditLog.UserName
        };
    }

    /// <summary>
    /// 异步获取带用户名的实体变更列表。
    /// </summary>
    /// <param name="entityId">实体标识。</param>
    /// <param name="entityTypeFullName">实体类型完整名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>带用户名的实体变更列表。</returns>
    public virtual async Task<List<EntityChangeWithUsername>> GetEntityChangesWithUsernameAsync(
        string entityId,
        string entityTypeFullName,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Set<EntityChange>()
                            .AsNoTracking()
                            .IncludeDetails()
                            .Where(x => x.EntityId == entityId && x.EntityTypeFullName == entityTypeFullName);

        return await (from e in query
                      join auditLog in dbContext.AuditLogs on e.AuditLogId equals auditLog.Id
                      select new EntityChangeWithUsername { EntityChange = e, UserName = auditLog.UserName })
                    .OrderByDescending(x => x.EntityChange.ChangeTime).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 获取实体变更查询。
    /// </summary>
    /// <param name="auditLogId">auditLogId。</param>
    /// <param name="startTime">开始时间。</param>
    /// <param name="endTime">结束时间。</param>
    /// <param name="changeType">变更类型。</param>
    /// <param name="entityId">实体标识。</param>
    /// <param name="entityTypeFullName">实体类型完整名称。</param>
    /// <param name="includeDetails">是否包含详情。</param>
    /// <returns>实体变更查询。</returns>
    protected virtual async Task<IQueryable<EntityChange>> GetEntityChangeListQueryAsync(
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string? entityId = null,
        string? entityTypeFullName = null,
        bool includeDetails = false)
    {
        return (await GetDbContextAsync())
            .Set<EntityChange>()
            .AsNoTracking()
            .IncludeDetails(includeDetails)
            .WhereIf(auditLogId.HasValue, e => e.AuditLogId == auditLogId)
            .WhereIf(startTime.HasValue, e => e.ChangeTime >= startTime)
            .WhereIf(endTime.HasValue, e => e.ChangeTime <= endTime)
            .WhereIf(changeType.HasValue, e => e.ChangeType == changeType)
            .WhereIf(!string.IsNullOrWhiteSpace(entityId), e => e.EntityId == entityId)
            .WhereIf(!string.IsNullOrWhiteSpace(entityTypeFullName), e => e.EntityTypeFullName!.Contains(entityTypeFullName!));
    }
}
