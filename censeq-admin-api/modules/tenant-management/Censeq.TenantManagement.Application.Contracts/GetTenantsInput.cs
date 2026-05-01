using Volo.Abp.Application.Dtos;

namespace Censeq.TenantManagement;

public class GetTenantsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 可选名称过滤；须为可空类型，否则在启用 NRT 时未传 query 会触发「Filter 必填」模型校验。
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 是否包含已删除的租户（仅平台管理员可用）。
    /// </summary>
    public bool IncludeDeleted { get; set; }
}
