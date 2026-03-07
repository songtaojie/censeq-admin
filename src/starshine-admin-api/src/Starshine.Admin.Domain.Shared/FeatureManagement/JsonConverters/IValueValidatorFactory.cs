using Volo.Abp.Validation.StringValues;

namespace Starshine.Admin.FeatureManagement.JsonConverters;

public interface IValueValidatorFactory
{
    bool CanCreate(string name);

    IValueValidator Create();
}
