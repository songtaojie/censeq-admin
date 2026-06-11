namespace Censeq.Framework.AspNetCore.SignalR.Dto;

/// <summary>
/// 管理员强制指定连接下线时提交的参数。
/// </summary>
public class ForceOfflineInput
{
    /// <summary>
    /// 需要强制下线的 SignalR 连接 ID。
    /// </summary>
    public string ConnectionId { get; set; } = string.Empty;

    /// <summary>
    /// 推送给客户端的下线原因，为空时使用默认提示。
    /// </summary>
    public string? Reason { get; set; }
}