using System;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予状态查询请求。
/// </summary>
public class IsGrantedRequest
{
    /// <summary>
    /// 用户标识。
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// 需要检查的权限名称集合。
    /// </summary>
    public string[]? PermissionNames { get; set; }
}
