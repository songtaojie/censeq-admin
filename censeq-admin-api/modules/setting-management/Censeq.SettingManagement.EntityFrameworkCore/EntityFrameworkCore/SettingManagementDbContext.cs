using Censeq.SettingManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace Censeq.SettingManagement.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(CenseqSettingManagementDbProperties.ConnectionStringName)]
public class SettingManagementDbContext : AbpDbContext<SettingManagementDbContext>, ISettingManagementDbContext
{
    public DbSet<Setting> Settings { get; set; }
    public DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; set; }

    public SettingManagementDbContext(DbContextOptions<SettingManagementDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureSettingManagement();
    }
}
