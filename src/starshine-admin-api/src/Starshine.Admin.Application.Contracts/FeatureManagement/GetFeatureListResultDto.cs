using System.Collections.Generic;

namespace Starshine.Admin.FeatureManagement;

public class GetFeatureListResultDto
{
    public List<FeatureGroupDto> Groups { get; set; }
}
