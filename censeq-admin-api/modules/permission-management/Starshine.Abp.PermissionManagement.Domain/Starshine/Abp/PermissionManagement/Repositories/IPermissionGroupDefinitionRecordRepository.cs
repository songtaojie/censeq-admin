using Censeq.Abp.Domain.Repositories;
using Censeq.Abp.PermissionManagement.Entities;

namespace Censeq.Abp.PermissionManagement.Repositories;

/// <summary>
/// 权限组定义记录存储库
/// </summary>
public interface IPermissionGroupDefinitionRecordRepository : IBasicRepository<PermissionGroupDefinitionRecord, Guid>
{

}