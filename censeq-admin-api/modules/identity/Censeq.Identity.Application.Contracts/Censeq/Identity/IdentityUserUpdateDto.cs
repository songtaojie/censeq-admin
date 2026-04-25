using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace Censeq.Identity;

/// <summary>
/// 更新用户 DTO
/// </summary>
public class IdentityUserUpdateDto : IdentityUserCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public string? Password { get; set; }

    /// <summary>
    /// 并发戳
    /// </summary>
    public string? ConcurrencyStamp { get; set; }
}
