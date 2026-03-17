using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;
using Censeq.Identity;
using Censeq.Account.Localization;

namespace Censeq.Account;

public class ChangePasswordInput : IValidatableObject
{
    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public string NewPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CurrentPassword == NewPassword)
        {
            var localizer = validationContext.GetRequiredService<IStringLocalizer<AccountResource>>();

            yield return new ValidationResult(
                localizer["NewPasswordSameAsOld"],
                new[] { nameof(CurrentPassword), nameof(NewPassword) }
            );
        }
    }
}
