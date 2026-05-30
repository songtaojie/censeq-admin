using Censeq.PermissionManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限组记录仓储。
/// </summary>
public interface IPermissionGroupRepository : IBasicRepository<PermissionGroup, Guid>
{
}
