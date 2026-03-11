using System.Collections.Generic;
using Censeq.FeatureManagement.JsonConverters;
using Volo.Abp.Validation.StringValues;

namespace Censeq.FeatureManagement;

public class ValueValidatorFactoryOptions
{
    public HashSet<IValueValidatorFactory> ValueValidatorFactory { get; }

    public ValueValidatorFactoryOptions()
    {
        ValueValidatorFactory = new HashSet<IValueValidatorFactory>
        {
            new ValueValidatorFactory<AlwaysValidValueValidator>("NULL"),
            new ValueValidatorFactory<BooleanValueValidator>("BOOLEAN"),
            new ValueValidatorFactory<NumericValueValidator>("NUMERIC"),
            new ValueValidatorFactory<StringValueValidator>("STRING")
        };
    }
}
