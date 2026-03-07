using Censeq.Abp.Domain.Repositories;
using Censeq.Abp.PermissionManagement.Entities;

namespace Censeq.Abp.PermissionManagement.Repositories;

/// <summary>
/// Ȩ�޶����¼�ִ�
/// </summary>
public interface IPermissionDefinitionRecordRepository : IBasicRepository<PermissionDefinitionRecord, Guid>
{
    /// <summary>
    /// �������ֻ�ȡȨ�޶����¼
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PermissionDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}