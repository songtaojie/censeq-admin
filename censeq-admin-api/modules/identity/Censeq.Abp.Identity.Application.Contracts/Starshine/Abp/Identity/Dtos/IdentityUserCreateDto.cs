using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Validation;

namespace Censeq.Abp.Identity.Dtos;
/// <summary>
/// ��֤�û�����Dto
/// </summary>
public class IdentityUserCreateDto : IdentityUserCreateOrUpdateDtoBase
{
    /// <summary>
    /// 
    /// </summary>
    [DisableAuditing]
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public required string Password { get; set; }
}
