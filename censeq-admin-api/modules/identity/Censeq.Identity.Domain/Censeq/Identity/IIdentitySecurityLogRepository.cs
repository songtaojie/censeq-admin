using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Identity;

/// <summary>
/// I身份安全日志仓储接口
/// </summary>
public interface IIdentitySecurityLogRepository : IBasicRepository<IdentitySecurityLog, Guid>
{
    Task<List<IdentitySecurityLog>> GetListAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? applicationName = null,
        string? identity = null,
        string? action = null,
        Guid? userId = null,
        string? userName = null,
        string? clientId = null,
        string? correlationId = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? applicationName = null,
        string? identity = null,
        string? action = null,
        Guid? userId = null,
        string? userName = null,
        string? clientId = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    Task<IdentitySecurityLog> GetByUserIdAsync(
        Guid id,
        Guid userId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);
}
