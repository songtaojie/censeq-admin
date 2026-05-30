using Censeq.PermissionManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予记录仓储。
/// </summary>
public interface IPermissionGrantRepository : IBasicRepository<PermissionGrant, Guid>
{
    /// <summary>
    /// 查找指定权限和提供者对应的授权记录。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权记录不存在时返回 null。</returns>
    Task<PermissionGrant?> FindAsync(string name, string providerName, string providerKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定提供者标识下的全部授权记录。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>权限授予记录列表。</returns>
    Task<List<PermissionGrant>> GetListAsync(string providerName, string providerKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定提供者标识下某些权限的授权记录。
    /// </summary>
    /// <param name="names">权限名称集合。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>权限授予记录列表。</returns>
    Task<List<PermissionGrant>> GetListAsync(string[] names, string providerName, string providerKey, CancellationToken cancellationToken = default);
}
