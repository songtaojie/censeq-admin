using System.Threading.Tasks;

namespace Censeq.FeatureManagement;

public interface IStaticFeatureSaver
{
    Task SaveAsync();
}
