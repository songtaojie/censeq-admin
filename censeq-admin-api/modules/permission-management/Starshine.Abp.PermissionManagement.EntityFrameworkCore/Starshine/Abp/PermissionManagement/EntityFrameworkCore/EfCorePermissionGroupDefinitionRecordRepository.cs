using Censeq.Abp.Domain.Repositories.EntityFrameworkCore;
using Censeq.Abp.EntityFrameworkCore;
using Censeq.Abp.PermissionManagement.Entities;
using Censeq.Abp.PermissionManagement.Repositories;
using Volo.Abp.DependencyInjection;

namespace Censeq.Abp.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 
/// </summary>
public class EfCorePermissionGroupDefinitionRecordRepository :
    EfCoreRepository<IPermissionManagementDbContext, PermissionGroupDefinitionRecord, Guid>,
    IPermissionGroupDefinitionRecordRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="abpLazyServiceProvider"></param>
    public EfCorePermissionGroupDefinitionRecordRepository(
        IDbContextProvider<IPermissionManagementDbContext> dbContextProvider,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(dbContextProvider, abpLazyServiceProvider)
    {
    }
}