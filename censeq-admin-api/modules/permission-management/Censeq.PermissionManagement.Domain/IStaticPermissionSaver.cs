using System.Threading.Tasks;

namespace Censeq.PermissionManagement;

public interface IStaticPermissionSaver
{
    Task SaveAsync();
}
