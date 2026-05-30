using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 权限定义记录 EF Core 仓储实现。
/// </summary>
public class EfCorePermissionDefinitionRecordRepository : EfCoreRepository<IPermissionManagementDbContext, PermissionDefinitionRecord, Guid>, IPermissionDefinitionRecordRepository
{
    /// <summary>
    /// 初始化权限定义记录仓储。
    /// </summary>
    /// <param name="dbContextProvider">权限管理 DbContext 提供器。</param>
    public EfCorePermissionDefinitionRecordRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// 根据权限名称查找权限定义记录。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>权限定义记录不存在时返回 null。</returns>
    public virtual async Task<PermissionDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
