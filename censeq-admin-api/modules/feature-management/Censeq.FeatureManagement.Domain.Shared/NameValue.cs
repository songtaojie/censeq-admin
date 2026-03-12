using System;

namespace Censeq.FeatureManagement;

[Serializable]
public class NameValue
{
    public string Name { get; set; } = default!;

    public string? Value { get; set; }

    public NameValue()
    {
    }

    public NameValue(string name, string? value)
    {
        Name = name;
        Value = value;
    }
}
