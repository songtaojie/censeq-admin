using Censeq.PermissionManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限定义记录仓储。
/// </summary>
public interface IPermissionDefinitionRecordRepository : IBasicRepository<PermissionDefinitionRecord, Guid>
{
    /// <summary>
    /// 根据权限名称查找权限定义记录。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>权限定义记录不存在时返回 null。</returns>
    Task<PermissionDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
