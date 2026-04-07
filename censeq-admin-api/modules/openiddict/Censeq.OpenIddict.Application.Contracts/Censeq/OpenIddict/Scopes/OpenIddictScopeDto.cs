using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Scopes;

public class OpenIddictScopeDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 作用域名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
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
