using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;

namespace Censeq.FeatureManagement;

public class FeatureStore : IFeatureStore, ITransientDependency
{
    protected IFeatureManagementStore FeatureManagementStore { get; }

    public FeatureStore(IFeatureManagementStore featureManagementStore)
    {
        FeatureManagementStore = featureManagementStore;
    }

    public virtual async Task<string?> GetOrNullAsync(
        string name,
        string? providerName,
        string? providerKey)
    {
        return await FeatureManagementStore.GetOrNullAsync(name, providerName ?? string.Empty, providerKey ?? string.Empty);
    }
}
