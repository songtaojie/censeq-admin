using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Scopes;

public class GetOpenIddictScopesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }
}
