using System;
using System.Security.Claims;
using JetBrains.Annotations;

namespace Censeq.Abp.Identity;

/// <summary>
/// ��ʾ�����ɫ�������û���������
/// </summary>
public class IdentityRoleClaim : IdentityClaim
{
    /// <summary>
    /// ��ȡ���������������ؽ�ɫ��������
    /// </summary>
    public virtual Guid RoleId { get; protected set; }

    /// <summary>
    /// ��ʼ�� <see cref="IdentityRoleClaim"/> �����ʵ����
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <param name="claim"></param>
    /// <param name="tenantId"></param>
    protected internal IdentityRoleClaim(Guid id, Guid roleId, [NotNull] Claim claim,Guid? tenantId)
        : base(id, claim,tenantId)
    {
        RoleId = roleId;
    }

    /// <summary>
    /// ��ʼ�� <see cref="IdentityRoleClaim"/> �����ʵ����
    /// </summary>
    /// <param name="id"></param>
    /// <param name="roleId"></param>
    /// <param name="claimType"></param>
    /// <param name="claimValue"></param>
    /// <param name="tenantId"></param>
    public IdentityRoleClaim(Guid id,Guid roleId,[NotNull] string claimType,string claimValue,Guid? tenantId)
        : base(id,claimType,claimValue,tenantId)
    {
        RoleId = roleId;
    }
}
