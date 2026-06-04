using System.Collections.Generic;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予状态查询响应。
/// </summary>
public class IsGrantedResponse
{
    /// <summary>
    /// 用户标识。
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// 权限名称及其授予状态。
    /// </summary>
    public Dictionary<string, bool> Permissions { get; set; } = [];
}
