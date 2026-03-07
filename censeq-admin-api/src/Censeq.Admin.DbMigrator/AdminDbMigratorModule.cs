using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Censeq.Admin.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(StarshineAdminEntityFrameworkCoreModule),
    typeof(StarshineAdminApplicationContractsModule)
    )]
public class AdminDbMigratorModule : AbpModule
{
}
