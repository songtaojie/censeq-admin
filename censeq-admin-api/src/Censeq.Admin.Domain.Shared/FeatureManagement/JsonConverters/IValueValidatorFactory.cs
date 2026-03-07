using Volo.Abp.Validation.StringValues;

namespace Censeq.Admin.FeatureManagement.JsonConverters;

public interface IValueValidatorFactory
{
    bool CanCreate(string name);

    IValueValidator Create();
}
