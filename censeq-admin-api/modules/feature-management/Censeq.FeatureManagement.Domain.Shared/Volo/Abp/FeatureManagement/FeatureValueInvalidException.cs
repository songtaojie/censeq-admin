using System;

namespace Censeq.FeatureManagement;

[Serializable]
public class FeatureValueInvalidException : BusinessException
{
    public FeatureValueInvalidException(string name) :
        base(FeatureManagementDomainErrorCodes.FeatureValueInvalid)
    {
        WithData("0", name);
    }
}
