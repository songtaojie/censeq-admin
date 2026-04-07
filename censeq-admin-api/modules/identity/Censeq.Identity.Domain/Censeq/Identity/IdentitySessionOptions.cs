using System;

namespace Censeq.Identity;

public class IdentitySessionOptions
{
    /// <summary>
    /// 是否启用会话管理功能
    /// 默认值：true
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 最大空闲时间，超过此时间未活动的会话将被视为过期
    /// 默认值：null（不限制）
    /// 示例：TimeSpan.FromDays(7) 表示 7 天无活动则过期
    /// </summary>
    public TimeSpan? MaxInactiveTime { get; set; }

    /// <summary>
    /// 会话最大存活时间，无论是否活动，超过此时间后会话都将过期
    /// 默认值：null（不限制）
    /// 示例：TimeSpan.FromDays(30) 表示会话最多存活 30 天
    /// </summary>
    public TimeSpan? MaxSessionLifetime { get; set; }

    /// <summary>
    /// 用户最多可同时拥有的会话数
    /// 默认值：null（不限制）
    /// </summary>
    public int? MaxConcurrentSessions { get; set; }

    /// <summary>
    /// 当达到最大会话数时，是否自动移除最旧的会话
    /// 默认值：true
    /// </summary>
    public bool AutoRemoveOldestSession { get; set; } = true;

    /// <summary>
    /// 是否保存设备信息（User-Agent）
    /// 默认值：true
    /// </summary>
    public bool SaveDeviceInfo { get; set; } = true;
}
