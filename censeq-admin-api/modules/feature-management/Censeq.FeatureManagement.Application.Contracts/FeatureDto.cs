using System.Collections.Generic;
using Volo.Abp.Validation.StringValues;

namespace Censeq.FeatureManagement;

public class FeatureDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Value { get; set; }

    public FeatureProviderDto Provider { get; set; }

    public string Description { get; set; }

    public string DefaultValue { get; set; }

    public bool IsVisibleToClients { get; set; }

    public bool IsAvailableToHost { get; set; }

    public List<string> AllowedProviders { get; set; }

    public IStringValueType ValueType { get; set; }

    public int Depth { get; set; }

    public string ParentName { get; set; }

    public FeatureDto()
    {
        AllowedProviders = new List<string>();
    }
}
