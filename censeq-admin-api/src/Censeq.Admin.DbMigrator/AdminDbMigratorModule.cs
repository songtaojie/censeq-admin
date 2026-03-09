using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Censeq.Admin.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CenseqAdminEntityFrameworkCoreModule),
    typeof(CenseqAdminApplicationContractsModule)
    )]
public class AdminDbMigratorModule : AbpModule
{
}
