using System.Threading.Tasks;

namespace Starshine.Admin.Data;

public interface IAdminDbSchemaMigrator
{
    Task MigrateAsync();
}
