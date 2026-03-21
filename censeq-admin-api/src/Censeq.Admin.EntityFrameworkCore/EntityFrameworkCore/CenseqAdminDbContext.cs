using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.Admin.EntityFrameworkCore;

[ConnectionStringName(ConnectionStrings.DefaultConnectionStringName)]
public class CenseqAdminDbContext(DbContextOptions<CenseqAdminDbContext> options) : AbpDbContext<CenseqAdminDbContext>(options){
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
