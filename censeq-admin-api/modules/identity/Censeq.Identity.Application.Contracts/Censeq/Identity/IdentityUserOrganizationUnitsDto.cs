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

    /// <summary>
    /// 主组织机构标识。为空时默认使用 OrganizationUnitIds 中的第一个。
    /// </summary>
    public Guid? PrimaryOrganizationUnitId { get; set; }
}
