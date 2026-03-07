using System;
using System.Collections.Generic;

namespace Censeq.Abp.Identity;
/// <summary>
/// �����û� ID �ͽ�ɫ����
/// </summary>
public class IdentityUserIdWithRoleNames
{
    /// <summary>
    /// ����id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ��ɫ����
    /// </summary>
    public IEnumerable<string> RoleNames { get; set; } = [];
}