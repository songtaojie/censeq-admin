namespace Censeq.Framework.AspNetCore.SignalR.Options;

/// <summary>
/// SignalR 模块配置项，对应 appsettings 中的 SignalR 节点。
/// </summary>
public class CenseqSignalROptions
{
    /// <summary>
    /// 是否启用详细错误信息。
    /// </summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// 服务端发送 keep-alive 心跳的间隔秒数。
    /// </summary>
    public int KeepAliveSeconds { get; set; } = 15;

    /// <summary>
    /// 客户端超时时间秒数。
    /// </summary>
    public int ClientTimeoutSeconds { get; set; } = 30;
}