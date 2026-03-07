using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Censeq.Abp.Domain.Repositories.EntityFrameworkCore;
using Censeq.Abp.EntityFrameworkCore;
using Censeq.Abp.PermissionManagement.Entities;
using Censeq.Abp.PermissionManagement.Repositories;
using Volo.Abp.DependencyInjection;

namespace Censeq.Abp.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// Ȩ���鶨��
/// </summary>
public class EfCorePermissionDefinitionRecordRepository :
    EfCoreRepository<IPermissionManagementDbContext, PermissionDefinitionRecord, Guid>,
    IPermissionDefinitionRecordRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContextProvider"></param>
    public EfCorePermissionDefinitionRecordRepository(
        IDbContextProvider<IPermissionManagementDbContext> dbContextProvider,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(dbContextProvider, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<PermissionDefinitionRecord?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}