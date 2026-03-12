using System;
using Censeq.FeatureManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.FeatureManagement;

public interface IFeatureGroupDefinitionRecordRepository : IBasicRepository<FeatureGroupDefinitionRecord, Guid>
{

}
