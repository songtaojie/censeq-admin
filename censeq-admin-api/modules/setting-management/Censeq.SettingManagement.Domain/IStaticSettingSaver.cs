using System.Threading.Tasks;

namespace Censeq.SettingManagement;

public interface IStaticSettingSaver
{
    Task SaveAsync();
}
