using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Censeq.OpenIddict.Applications;

public class OpenIddictApplicationCreateDto
{
    /// <summary>
    /// 客户端ID (必需)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    [StringLength(200)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// 客户端类型 (confidential, public)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ClientType { get; set; } = "confidential";

    /// <summary>
    /// 应用类型 (web, native)
    /// </summary>
    [StringLength(50)]
    public string? ApplicationType { get; set; } = "web";

    /// <summary>
    /// 同意类型 (explicit, implicit, external)
    /// </summary>
    [StringLength(50)]
    public string? ConsentType { get; set; } = "explicit";

    /// <summary>
    /// 客户端密钥 (confidential类型必需)
    /// </summary>
    [StringLength(300)]
    public string? ClientSecret { get; set; }

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
