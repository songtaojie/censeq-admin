using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Applications;

public class GetOpenIddictApplicationsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 客户端类型
    /// </summary>
    public string? ClientType { get; set; }
}
