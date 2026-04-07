using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Censeq.OpenIddict.Applications;

public class OpenIddictApplicationUpdateDto
{
    /// <summary>
    /// 显示名称
    /// </summary>
    [StringLength(200)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// 客户端密钥 (设置则更新，留空则保持不变)
    /// </summary>
    [StringLength(300)]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// 同意类型 (explicit, implicit, external)
    /// </summary>
    [StringLength(50)]
    public string? ConsentType { get; set; }

    /// <summary>
    /// 回调地址列表
    /// </summary>
    public List<string> RedirectUris { get; set; } = new();

    /// <summary>
    /// 登出回调地址列表
    /// </summary>
    public List<string> PostLogoutRedirectUris { get; set; } = new();

    /// <summary>
    /// 权限列表
    /// </summary>
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// 要求列表
    /// </summary>
    public List<string> Requirements { get; set; } = new();

    /// <summary>
    /// 客户端URI
    /// </summary>
    [StringLength(500)]
    public string? ClientUri { get; set; }

    /// <summary>
    /// Logo URI
    /// </summary>
    [StringLength(500)]
    public string? LogoUri { get; set; }
}
