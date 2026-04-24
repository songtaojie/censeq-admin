using Volo.Abp.Domain.Entities;

namespace Censeq.Identity;

/// <summary>
/// 更新角色 DTO
/// </summary>
public class IdentityRoleUpdateDto : IdentityRoleCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    /// <summary>
    /// 并发戳
    /// </summary>
    public string ConcurrencyStamp { get; set; }
}
