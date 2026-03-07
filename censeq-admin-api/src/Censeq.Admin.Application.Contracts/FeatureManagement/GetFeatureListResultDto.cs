using System.Collections.Generic;

namespace Censeq.Admin.FeatureManagement;

public class GetFeatureListResultDto
{
    public List<FeatureGroupDto> Groups { get; set; }
}
