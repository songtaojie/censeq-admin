using Censeq.PermissionManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.PermissionManagement;

public interface IPermissionDefinitionRecordRepository : IBasicRepository<PermissionDefinitionRecord, Guid>
{
    Task<PermissionDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
