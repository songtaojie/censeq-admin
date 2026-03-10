using Censeq.SettingManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace Censeq.SettingManagement.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(CenseqSettingManagementDbProperties.ConnectionStringName)]
public interface ISettingManagementDbContext : IEfCoreDbContext
{
    DbSet<Setting> Settings { get; }

    DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; }
}
