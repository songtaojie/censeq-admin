using Censeq.Admin.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Admin.FeatureManagement;

public interface IFeatureDefinitionRecordRepository : IBasicRepository<FeatureDefinitionRecord, Guid>
{
    Task<FeatureDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}
