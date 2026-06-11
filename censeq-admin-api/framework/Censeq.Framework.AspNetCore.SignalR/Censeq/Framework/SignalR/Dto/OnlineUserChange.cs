namespace Censeq.Framework.AspNetCore.SignalR.Dto;

/// <summary>
/// 在线用户连接状态变更事件。
/// </summary>
/// <param name="User">发生变更的在线用户连接信息。</param>
/// <param name="Online">true 表示上线，false 表示离线。</param>
public sealed record OnlineUserChange(OnlineUserInfo User, bool Online);