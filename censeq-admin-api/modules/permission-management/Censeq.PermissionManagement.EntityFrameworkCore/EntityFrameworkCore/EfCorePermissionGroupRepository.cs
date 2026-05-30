using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 权限组 EF Core 仓储实现。
/// </summary>
public class EfCorePermissionGroupRepository : EfCoreRepository<IPermissionManagementDbContext, PermissionGroup, Guid>, IPermissionGroupRepository
{
    /// <summary>
    /// 初始化权限组仓储。
    /// </summary>
    /// <param name="dbContextProvider">权限管理 DbContext 提供器。</param>
    public EfCorePermissionGroupRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
