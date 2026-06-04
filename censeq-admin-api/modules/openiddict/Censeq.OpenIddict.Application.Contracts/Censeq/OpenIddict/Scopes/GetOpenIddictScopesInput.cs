using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域查询输入。
/// </summary>
public class GetOpenIddictScopesInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }
}
