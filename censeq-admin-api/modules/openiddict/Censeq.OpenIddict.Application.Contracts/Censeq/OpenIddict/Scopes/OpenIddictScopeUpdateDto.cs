using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Censeq.OpenIddict.Scopes;

public class OpenIddictScopeUpdateDto
{
    /// <summary>
    /// 显示名称
    /// </summary>
    [StringLength(200)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 资源列表
    /// </summary>
    public List<string> Resources { get; set; } = new();

    /// <summary>
    /// 属性
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = new();
}
