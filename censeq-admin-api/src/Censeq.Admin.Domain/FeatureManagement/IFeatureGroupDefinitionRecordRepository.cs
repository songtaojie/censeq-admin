using Censeq.Admin.Entities;
using System;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Admin.FeatureManagement;

public interface IFeatureGroupDefinitionRecordRepository : IBasicRepository<FeatureGroupDefinitionRecord, Guid>
{

}
