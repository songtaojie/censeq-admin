using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Localization;
using Volo.Abp;
using Censeq.Identity.Localization;

namespace Censeq.Identity;

public class CenseqIdentityResultException : BusinessException, ILocalizeErrorMessage
{
    public IdentityResult IdentityResult { get; }

    public CenseqIdentityResultException([NotNull] IdentityResult identityResult)
        : base(
            code: $"Censeq.Identity:{identityResult.Errors.First().Code}",
            message: identityResult.Errors.Select(err => err.Description).JoinAsString(", "))
    {
        IdentityResult = Check.NotNull(identityResult, nameof(identityResult));
    }

    public virtual string LocalizeMessage(LocalizationContext context)
    {
        var localizer = context.LocalizerFactory.Create<IdentityResource>();

        SetData(localizer);

        return IdentityResult.LocalizeErrors(localizer);
    }

    protected virtual void SetData(IStringLocalizer localizer)
    {
        var values = IdentityResult.GetValuesFromErrorMessage(localizer);

        for (var index = 0; index < values.Length; index++)
        {
            Data[index.ToString()] = values[index];
        }
    }
}
