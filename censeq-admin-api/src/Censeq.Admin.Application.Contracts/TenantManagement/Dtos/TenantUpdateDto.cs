using Volo.Abp.Domain.Entities;

namespace Censeq.Admin.TenantManagement.Dtos;
/// <summary>
/// 租户更新Dto
/// </summary>
public class TenantUpdateDto : TenantCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    /// <summary>
    /// 版本
    /// </summary>
    public required string ConcurrencyStamp { get; set; }
}
