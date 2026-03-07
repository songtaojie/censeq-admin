using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Abp.Identity;

/// <summary>
/// ��ʾ�û��ͽ�ɫ֮�����ϵ��
/// </summary>
public class IdentityUserRole : Entity, IMultiTenant
{
    /// <summary>
    /// �⻧id
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// ��ȡ���������ɫ��������û���������
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    /// <summary>
    /// ��ȡ���������û������Ľ�ɫ��������
    /// </summary>
    public virtual Guid RoleId { get; protected set; }

    /// <summary>
    /// ����һ�� <see cref="IdentityUserRole"/> ʵ����
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="tenantId"></param>
    protected internal IdentityUserRole(Guid userId, Guid roleId, Guid? tenantId)
    {
        UserId = userId;
        RoleId = roleId;
        TenantId = tenantId;
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <returns></returns>
    public override object[] GetKeys()
    {
        return [UserId, RoleId];
    }
}
