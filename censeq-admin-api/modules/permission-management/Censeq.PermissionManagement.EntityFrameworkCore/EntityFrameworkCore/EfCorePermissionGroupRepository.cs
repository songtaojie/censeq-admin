using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

public class EfCorePermissionGroupRepository : EfCoreRepository<IPermissionManagementDbContext, PermissionGroup, Guid>, IPermissionGroupRepository
{
    public EfCorePermissionGroupRepository(IDbContextProvider<IPermissionManagementDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}