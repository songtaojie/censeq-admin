using System;
using System.Collections.Generic;

namespace Censeq.Identity;

/// <summary>
/// 身份用户组织Units数据传输对象
/// </summary>
public class IdentityUserOrganizationUnitsDto
{
    /// <summary>
    /// List<Guid>
    /// </summary>
    public List<Guid> OrganizationUnitIds { get; set; } = new();
}
