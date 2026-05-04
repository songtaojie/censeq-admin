using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.LocalizationManagement;

public interface ILocalizationTextRepository : IRepository<LocalizationText, Guid>
{
    Task<LocalizationText?> FindAsync(
        string resourceName,
        string cultureName,
        string key,
        Guid? tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询指定资源+语言下的所有翻译条目（用于 DbLocalizationResourceContributor.Fill）
    /// </summary>
    Task<List<LocalizationText>> GetListAsync(
        string resourceName,
        string cultureName,
        Guid? tenantId,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? resourceName = null,
        string? cultureName = null,
        Guid? tenantId = null,
        string? filter = null,
        CancellationToken cancellationToken = default);

    Task<List<LocalizationText>> GetPagedListAsync(
        string? resourceName = null,
        string? cultureName = null,
        Guid? tenantId = null,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = 20,
        string? sorting = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询指定资源下所有存在翻译的语言名称（用于 GetSupportedCulturesAsync）
    /// </summary>
    Task<List<string>> GetDistinctCultureNamesAsync(
        string resourceName,
        Guid? tenantId,
        CancellationToken cancellationToken = default);
}
