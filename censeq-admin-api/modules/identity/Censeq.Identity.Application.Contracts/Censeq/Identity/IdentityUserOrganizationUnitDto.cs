namespace Censeq.Identity;

/// <summary>
/// 用户所属组织机构数据传输对象。
/// </summary>
public class IdentityUserOrganizationUnitDto : OrganizationUnitDto
{
    /// <summary>
    /// 是否为用户的主组织机构。
    /// </summary>
    public bool IsPrimary { get; set; }
}
