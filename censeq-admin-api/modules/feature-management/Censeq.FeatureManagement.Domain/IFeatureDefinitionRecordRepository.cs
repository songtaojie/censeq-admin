using System;
using System.Threading;
using System.Threading.Tasks;
using Censeq.FeatureManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.FeatureManagement;

public interface IFeatureDefinitionRecordRepository : IBasicRepository<FeatureDefinitionRecord, Guid>
{
    Task<FeatureDefinitionRecord?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}
