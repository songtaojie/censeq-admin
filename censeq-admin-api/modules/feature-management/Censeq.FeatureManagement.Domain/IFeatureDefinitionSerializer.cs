using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.FeatureManagement.Entities;
using JetBrains.Annotations;
using Volo.Abp.Features;

namespace Censeq.FeatureManagement;

public interface IFeatureDefinitionSerializer
{
    Task<(FeatureGroupDefinitionRecord[], FeatureDefinitionRecord[])> SerializeAsync(IEnumerable<FeatureGroupDefinition> featureGroups);

    Task<FeatureGroupDefinitionRecord> SerializeAsync(FeatureGroupDefinition featureGroup);

    Task<FeatureDefinitionRecord> SerializeAsync(FeatureDefinition feature, [CanBeNull] FeatureGroupDefinition featureGroup);
}
