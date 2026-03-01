using Starshine.Admin.FeatureManagement.JsonConverters;
using System.Collections.Generic;
using Volo.Abp.Validation.StringValues;

namespace Starshine.Admin.FeatureManagement;

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
