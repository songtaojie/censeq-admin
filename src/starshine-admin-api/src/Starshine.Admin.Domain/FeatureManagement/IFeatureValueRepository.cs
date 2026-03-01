using Starshine.Admin.Entities;
using System.Collections.Generic;
using Volo.Abp.Domain.Repositories;

namespace Starshine.Admin.FeatureManagement;

public interface IFeatureValueRepository : IBasicRepository<FeatureValue, Guid>
{
    Task<FeatureValue> FindAsync(
        string name,
        string providerName,
        string? providerKey,
        CancellationToken cancellationToken = default);

    Task<List<FeatureValue>> FindAllAsync(
        string name,
        string providerName,
        string? providerKey,
        CancellationToken cancellationToken = default);

    Task<List<FeatureValue>> GetListAsync(
        string? providerName,
        string? providerKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default);
}
