namespace Censeq.Framework.AspNetCore.SignalR.Dto;

/// <summary>
/// 当前在线用户的单个 SignalR 连接信息，一个浏览器标签页通常对应一条连接记录。
/// </summary>
public class OnlineUserInfo
{
    /// <summary>
    /// SignalR 当前连接 ID。
    /// </summary>
    public string ConnectionId { get; set; } = string.Empty;

    /// <summary>
    /// 登录用户 ID。
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 登录用户名。
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 用户显示名称。
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 当前租户 ID，宿主侧连接为空。
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 当前登录会话 ID，用于和身份模块会话数据关联。
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// 客户端 IP 地址。
    /// </summary>
    public string? Ip { get; set; }

    /// <summary>
    /// 客户端 User-Agent 信息。
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 连接建立时间。
    /// </summary>
    public DateTimeOffset ConnectedAt { get; set; }
}