using Censeq.PermissionManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.PermissionManagement;

public interface IPermissionGroupRepository : IBasicRepository<PermissionGroup, Guid>
{
}