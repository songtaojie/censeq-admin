using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Censeq.FeatureManagement.Entities;

namespace Censeq.FeatureManagement.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(CenseqFeatureManagementDbProperties.ConnectionStringName)]
public interface IFeatureManagementDbContext : IEfCoreDbContext
{
    DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; }

    DbSet<FeatureDefinitionRecord> Features { get; }

    DbSet<FeatureValue> FeatureValues { get; }
}
