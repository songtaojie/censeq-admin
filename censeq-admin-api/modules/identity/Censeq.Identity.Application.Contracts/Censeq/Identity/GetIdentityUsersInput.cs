using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 获取身份用户列表输入参数
/// </summary>
public class GetIdentityUsersInput : ExtensiblePagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 组织机构Id
    /// </summary>
    public Guid? OrganizationUnitId { get; set; }

    /// <summary>
    /// 组织机构Id列表
    /// </summary>
    public List<Guid>? OrganizationUnitIds { get; set; }
}
