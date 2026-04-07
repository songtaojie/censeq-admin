using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Applications;

public class OpenIddictApplicationDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 应用类型 (web, native)
    /// </summary>
    public string ApplicationType { get; set; } = string.Empty;

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 客户端密钥 (仅用于显示，返回空或提示)
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// 客户端类型 (confidential, public)
    /// </summary>
    public string ClientType { get; set; } = string.Empty;

    /// <summary>
    /// 同意类型 (explicit, implicit, external)
    /// </summary>
    public string? ConsentType { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string? DisplayName { get; set; }

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
    public string? ClientUri { get; set; }

    /// <summary>
    /// Logo URI
    /// </summary>
    public string? LogoUri { get; set; }

    /// <summary>
    /// 设置
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new();
}
