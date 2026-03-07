using Volo.Abp.Domain.Entities;

namespace Censeq.Abp.Identity.Dtos;
/// <summary>
/// 
/// </summary>
public class IdentityRoleUpdateDto : IdentityRoleCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    /// <summary>
    /// 并发标记
    /// </summary>
    public string ConcurrencyStamp { get; set; } = null!;
}
