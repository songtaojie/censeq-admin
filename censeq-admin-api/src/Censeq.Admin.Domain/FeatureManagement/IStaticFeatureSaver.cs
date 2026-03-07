using System.Threading.Tasks;

namespace Censeq.Admin.FeatureManagement;

public interface IStaticFeatureSaver
{
    Task SaveAsync();
}
