using Volo.Abp.Validation.StringValues;

namespace Censeq.FeatureManagement.JsonConverters;

public interface IValueValidatorFactory
{
    bool CanCreate(string name);

    IValueValidator Create();
}
