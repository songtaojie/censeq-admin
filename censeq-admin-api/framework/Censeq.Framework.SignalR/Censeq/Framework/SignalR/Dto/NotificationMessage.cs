namespace Censeq.Framework.SignalR.Dto;

/// <summary>
/// 服务端推送到前端的实时通知消息。
/// </summary>
public class NotificationMessage
{
    /// <summary>
    /// 通知唯一标识，默认在服务端生成。
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 通知标题。
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 通知正文内容。
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 通知类型，前端可据此展示不同样式。
    /// </summary>
    public string Type { get; set; } = "info";

    /// <summary>
    /// 通知创建时间，默认使用 UTC 时间。
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 业务扩展字段，用于携带跳转地址、业务单号等附加信息。
    /// </summary>
    public Dictionary<string, string?> ExtraProperties { get; set; } = [];
}
