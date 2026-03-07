using System.Threading.Tasks;

namespace Censeq.Admin.Data;

public interface IAdminDbSchemaMigrator
{
    Task MigrateAsync();
}
