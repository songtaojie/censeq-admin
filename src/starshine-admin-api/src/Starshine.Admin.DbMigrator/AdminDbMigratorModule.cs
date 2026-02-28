using Starshine.Admin.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Starshine.Admin.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AdminEntityFrameworkCoreModule),
    typeof(StarshineAdminApplicationContractsModule)
    )]
public class AdminDbMigratorModule : AbpModule
{
}
