using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;
using Censeq.Identity;
using Censeq.Account.Localization;

namespace Censeq.Account;

/// <summary>
/// 修改密码输入。
/// </summary>
public class ChangePasswordInput : IValidatableObject
{
    /// <summary>
    /// 当前密码。
    /// </summary>
    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// 新密码。
    /// </summary>
    [Required]
    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// 验证输入数据。
    /// </summary>
    /// <param name="validationContext">validation 上下文。</param>
    /// <returns>返回结果。</returns>
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
