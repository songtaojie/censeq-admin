using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 角色声明DTO
/// </summary>
public class IdentityRoleClaimDto : EntityDto<Guid>
{
    /// <summary>
    /// 声明类型
    /// </summary>
    public string ClaimType { get; set; }

    /// <summary>
    /// 声明值
    /// </summary>
    public string ClaimValue { get; set; }
}

/// <summary>
/// 创建角色声明DTO
/// </summary>
public class IdentityRoleClaimCreateDto
{
    /// <summary>
    /// 声明类型
    /// </summary>
    public string ClaimType { get; set; }

    /// <summary>
    /// 声明值
    /// </summary>
    public string ClaimValue { get; set; }
}

/// <summary>
/// 角色声明列表结果
/// </summary>
public class IdentityRoleClaimListDto : ListResultDto<IdentityRoleClaimDto>
{
    public IdentityRoleClaimListDto()
    {
    }

    public IdentityRoleClaimListDto(System.Collections.Generic.IReadOnlyList<IdentityRoleClaimDto> items)
        : base(items)
    {
    }
}
