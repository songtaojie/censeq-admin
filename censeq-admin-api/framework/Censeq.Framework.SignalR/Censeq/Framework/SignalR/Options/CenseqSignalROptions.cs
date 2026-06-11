namespace Censeq.Framework.SignalR.Options;

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

    /// <summary>
    /// Redis 连接字符串，后续启用分布式 SignalR 背板时使用。
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// Redis 背板通道前缀，避免不同系统之间的消息串扰。
    /// </summary>
    public string? RedisChannelPrefix { get; set; } = "censeq:signalr";
}
