using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

public class EfCorePermissionGroupDefinitionRecordRepository : EfCoreRepository<IPermissionManagementDbContext, PermissionGroupDefinitionRecord, Guid>, IPermissionGroupDefinitionRecordRepository
{
    public EfCorePermissionGroupDefinitionRecordRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
