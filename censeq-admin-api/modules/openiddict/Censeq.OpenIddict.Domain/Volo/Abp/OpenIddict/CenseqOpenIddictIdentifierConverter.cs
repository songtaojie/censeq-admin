using System;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

public class CenseqOpenIddictIdentifierConverter : ITransientDependency
{
    public virtual Guid FromString(string identifier)
    {
        return string.IsNullOrEmpty(identifier) ? default : Guid.Parse(identifier);
    }

    public virtual string ToString(Guid identifier)
    {
        return identifier.ToString("D");
    }
}
