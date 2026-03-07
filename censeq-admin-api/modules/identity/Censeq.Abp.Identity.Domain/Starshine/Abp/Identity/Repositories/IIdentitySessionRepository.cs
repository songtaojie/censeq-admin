using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Abp.Identity;

/// <summary>
/// IIdentity �Ự�洢��
/// </summary>
public interface IIdentitySessionRepository : IBasicRepository<IdentitySession, Guid>
{
    /// <summary>
    /// ��ȡָ���Ự
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentitySession?> FindAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡָ���Ự
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IdentitySession> GetAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ָ���Ự
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// ����ָ���Ự
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> ExistAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sorting"></param>
    /// <param name="maxResultCount"></param>
    /// <param name="skipCount"></param>
    /// <param name="userId"></param>
    /// <param name="device"></param>
    /// <param name="clientId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IdentitySession>> GetListAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        Guid? userId = null,
        string? device = null,
        string? clientId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="device"></param>
    /// <param name="clientId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> GetCountAsync(
        Guid? userId = null,
        string? device = null,
        string? clientId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="exceptSessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAllAsync(Guid userId, Guid? exceptSessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="device"></param>
    /// <param name="exceptSessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAllAsync(Guid userId, string device, Guid? exceptSessionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inactiveTimeSpan"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAllAsync(TimeSpan inactiveTimeSpan, CancellationToken cancellationToken = default);
}
