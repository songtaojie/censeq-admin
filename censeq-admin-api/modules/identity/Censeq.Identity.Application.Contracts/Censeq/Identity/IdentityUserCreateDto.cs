using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;

namespace Censeq.Identity;

/// <summary>
/// 创建用户 DTO
/// </summary>
public class IdentityUserCreateDto : IdentityUserCreateOrUpdateDtoBase
{
    [DisableAuditing]
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public string Password { get; set; }
}
