using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 权限授予记录 EF Core 仓储实现。
/// </summary>
public class EfCorePermissionGrantRepository : EfCoreRepository<IPermissionManagementDbContext, PermissionGrant, Guid>, IPermissionGrantRepository
{
    /// <summary>
    /// 初始化权限授予记录仓储。
    /// </summary>
    /// <param name="dbContextProvider">权限管理 DbContext 提供器。</param>
    public EfCorePermissionGrantRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// 查找指定权限和提供者对应的授权记录。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>授权记录不存在时返回 null。</returns>
    public virtual async Task<PermissionGrant?> FindAsync(string name, string providerName, string providerKey, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 获取指定提供者标识下的全部授权记录。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>权限授予记录列表。</returns>
    public virtual async Task<List<PermissionGrant>> GetListAsync(string providerName, string providerKey, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(s => s.ProviderName == providerName && s.ProviderKey == providerKey)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 获取指定提供者标识下某些权限的授权记录。
    /// </summary>
    /// <param name="names">权限名称集合。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>权限授予记录列表。</returns>
    public virtual async Task<List<PermissionGrant>> GetListAsync(string[] names, string providerName, string providerKey, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(s => names.Contains(s.Name) && s.ProviderName == providerName && s.ProviderKey == providerKey)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
