using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志存储，负责持久化审计信息。
/// </summary>
public class AuditingStore : IAuditingStore, ITransientDependency
{
    /// <summary>
    /// 日志记录器。
    /// </summary>
    public ILogger<AuditingStore> Logger { get; set; }
    /// <summary>
    /// 审计日志仓储。
    /// </summary>
    protected IAuditLogRepository AuditLogRepository { get; }
    /// <summary>
    /// 工作单元管理器。
    /// </summary>
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    /// <summary>
    /// 审计日志配置项。
    /// </summary>
    protected AbpAuditingOptions Options { get; }
    /// <summary>
    /// 审计日志信息转换器。
    /// </summary>
    protected IAuditLogInfoToAuditLogConverter Converter { get; }
    /// <summary>
    /// 初始化 AuditingStore 实例。
    /// </summary>
    /// <param name="auditLogRepository">审计日志仓储。</param>
    /// <param name="unitOfWorkManager">unitOfWorkManager。</param>
    /// <param name="options">配置项。</param>
    /// <param name="converter">converter。</param>
    public AuditingStore(
        IAuditLogRepository auditLogRepository,
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<AbpAuditingOptions> options,
        IAuditLogInfoToAuditLogConverter converter)
    {
        AuditLogRepository = auditLogRepository;
        UnitOfWorkManager = unitOfWorkManager;
        Converter = converter;
        Options = options.Value;

        Logger = NullLogger<AuditingStore>.Instance;
    }

    /// <summary>
    /// 异步保存审计日志。
    /// </summary>
    /// <param name="auditInfo">auditInfo。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task SaveAsync(AuditLogInfo auditInfo)
    {
        if (!Options.HideErrors)
        {
            await SaveLogAsync(auditInfo);
            return;
        }

        try
        {
            await SaveLogAsync(auditInfo);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Could not save the audit log object: " + Environment.NewLine + auditInfo.ToString());
            Logger.LogException(ex, LogLevel.Error);
        }
    }

    /// <summary>
    /// 异步保存审计日志。
    /// </summary>
    /// <param name="auditInfo">auditInfo。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task SaveLogAsync(AuditLogInfo auditInfo)
    {
        using (var uow = UnitOfWorkManager.Begin(true))
        {
            await AuditLogRepository.InsertAsync(await Converter.ConvertAsync(auditInfo));
            await uow.CompleteAsync();
        }
    }
}
