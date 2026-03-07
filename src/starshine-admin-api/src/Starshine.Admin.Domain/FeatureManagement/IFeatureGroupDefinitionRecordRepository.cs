using Starshine.Admin.Entities;
using System;
using Volo.Abp.Domain.Repositories;

namespace Starshine.Admin.FeatureManagement;

public interface IFeatureGroupDefinitionRecordRepository : IBasicRepository<FeatureGroupDefinitionRecord, Guid>
{

}
