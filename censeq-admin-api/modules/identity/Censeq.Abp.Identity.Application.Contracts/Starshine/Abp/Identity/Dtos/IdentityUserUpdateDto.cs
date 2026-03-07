using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;

namespace Censeq.Abp.Identity.Dtos;
/// <summary>
/// ��֤�û�����DTO
/// </summary>
public class IdentityUserUpdateDto : IdentityUserCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    /// <summary>
    /// ����
    /// </summary>
    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    public required string Password { get; set; }

    /// <summary>
    /// �������
    /// </summary>
    public required string ConcurrencyStamp { get; set; }
}
