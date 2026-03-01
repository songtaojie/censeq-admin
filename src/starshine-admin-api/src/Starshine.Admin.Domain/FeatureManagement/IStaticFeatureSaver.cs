using System.Threading.Tasks;

namespace Starshine.Admin.FeatureManagement;

public interface IStaticFeatureSaver
{
    Task SaveAsync();
}
