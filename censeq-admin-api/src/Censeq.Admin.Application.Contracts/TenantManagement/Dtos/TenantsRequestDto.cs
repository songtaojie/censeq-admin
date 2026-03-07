using Volo.Abp.Application.Dtos;

namespace Censeq.Admin.TenantManagement.Dtos;

/// <summary>
/// 获取租户请求
/// </summary>
public class TenantsRequestDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }
}
