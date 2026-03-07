using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Abp.Identity;

/// <summary>
/// �����û���������֤���ơ�
/// </summary>
public class IdentityUserToken : Entity, IMultiTenant
{
    /// <summary>
    /// �⻧id
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// ��ȡ���������������û���������
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    /// <summary>
    /// ��ȡ�����ô����������� LoginProvider��
    /// </summary>
    public virtual string LoginProvider { get; protected set; }

    /// <summary>
    /// ��ȡ���������Ƶ����ơ�
    /// </summary>
    public virtual string Name { get; protected set; } 

    /// <summary>
    /// ��ȡ����������ֵ��
    /// </summary>
    public virtual string? Value { get; set; }

    /// <summary>
    /// ���캯����
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="tenantId"></param>
    protected internal IdentityUserToken(Guid userId,string loginProvider,string name,string? value,Guid? tenantId)
    {
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(name, nameof(name));
        UserId = userId;
        LoginProvider = loginProvider;
        Name = name;
        Value = value;
        TenantId = tenantId;
    }

    /// <summary>
    /// ��ȡ��ʵ���������
    /// </summary>
    /// <returns></returns>
    public override object[] GetKeys()
    {
        return [UserId, LoginProvider, Name];
    }
}
