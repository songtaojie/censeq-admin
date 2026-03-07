using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Starshine.Admin.Data;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Starshine.Admin.EntityFrameworkCore;

public class EfCoreAdminDbSchemaMigrator(
    IServiceProvider serviceProvider) : IAdminDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the AdminDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<StarshineAdminDbContext>()
            .Database
            .MigrateAsync();
    }
}
