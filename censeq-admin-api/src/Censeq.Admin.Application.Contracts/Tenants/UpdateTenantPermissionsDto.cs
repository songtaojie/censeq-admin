using System.Collections.Generic;

namespace Censeq.Admin.Tenants;

/// <summary>
/// 更新租户授权范围请求。
/// </summary>
public class UpdateTenantPermissionsDto
{
    /// <summary>
    /// 平台向该租户开放的权限名称列表（全量替换）。
    /// </summary>
    public List<string> GrantedPermissions { get; set; } = new();
}
