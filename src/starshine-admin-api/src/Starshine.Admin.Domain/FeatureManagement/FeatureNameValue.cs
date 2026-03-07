using System;
using Volo.Abp;

namespace Starshine.Admin.FeatureManagement;

[Serializable]
public class FeatureNameValue : NameValue
{
    public FeatureNameValue()
    {

    }

    public FeatureNameValue(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
