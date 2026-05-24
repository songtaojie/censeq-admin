using System.ComponentModel.DataAnnotations;
using Censeq.Framework.Core;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Censeq.Identity;

/// <summary>
/// 身份角色创建Or更新数据传输对象基类
/// </summary>
public class IdentityRoleCreateOrUpdateDtoBase : ExtensibleObject
{
    [Required]
    [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxNameLength))]
    [Display(Name = "RoleName")]
    public string Name { get; set; }

    [Required]
    [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxCodeLength))]
    [Display(Name = "RoleCode")]
    public string Code { get; set; }

    public bool IsDefault { get; set; }

    public bool IsPublic { get; set; }

    public CommonStatus Status { get; set; } = CommonStatus.Enabled;

    [StringLength(512)]
    public string? Remark { get; set; }

    protected IdentityRoleCreateOrUpdateDtoBase() : base(false)
    {

    }
}
