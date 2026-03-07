using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.Abp.Identity;

/// <summary>
/// ��������ϵͳ�еĽ�ɫ
/// </summary>
public class IdentityRole : AggregateRoot<Guid>, IMultiTenant, IHasEntityVersion
{
    /// <summary>
    /// �⻧uid
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    ///��ȡ�����ô˽�ɫ�����ơ�
    /// </summary>
    public virtual string Name { get; protected internal set; }

    /// <summary>
    /// ��ȡ�����ô˽�ɫ�Ĺ淶�����ơ�
    /// </summary>
    [DisableAuditing]
    public virtual string NormalizedName { get; protected internal set; } 

    /// <summary>
    /// �˽�ɫ�������ĵ������ԡ�
    /// </summary>
    public virtual ICollection<IdentityRoleClaim> Claims { get; protected set; }

    /// <summary>
    /// Ĭ�Ͻ�ɫ���Զ���������û�
    /// </summary>
    public virtual bool IsDefault { get; set; }

    /// <summary>
    ///��̬��ɫ�޷�ɾ��/������
    /// </summary>
    public virtual bool IsStatic { get; set; }

    /// <summary>
    /// �û����Կ��������û��Ĺ�����ɫ
    /// </summary>
    public virtual bool IsPublic { get; set; }

    /// <summary>
    ///ÿ��ʵ�巢���仯ʱ���汾ֵ�ͻ����ӡ�
    /// </summary>
    public virtual int EntityVersion { get; protected set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="tenantId"></param>
    public IdentityRole(Guid id, string name, Guid? tenantId = null)
    {
        Check.NotNull(name, nameof(name));
        Id = id;
        Name = name;
        TenantId = tenantId;
        NormalizedName = name.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
        Claims = new Collection<IdentityRoleClaim>();
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="guidGenerator"></param>
    /// <param name="claim"></param>
    public virtual void AddClaim(IGuidGenerator guidGenerator, Claim claim)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claim, nameof(claim));
        Claims.Add(new IdentityRoleClaim(guidGenerator.Create(), Id, claim, TenantId));
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="guidGenerator"></param>
    /// <param name="claims"></param>
    public virtual void AddClaims(IGuidGenerator guidGenerator, IEnumerable<Claim> claims)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claims, nameof(claims));
        foreach (var claim in claims)
        {
            AddClaim(guidGenerator, claim);
        }
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public virtual IdentityRoleClaim? FindClaim(Claim claim)
    {
        Check.NotNull(claim, nameof(claim));
        return Claims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
    }

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="claim"></param>
    public virtual void RemoveClaim(Claim claim)
    {
        Check.NotNull(claim, nameof(claim));
        Claims.RemoveAll(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
    }

    /// <summary>
    /// �ı�����
    /// </summary>
    /// <param name="name"></param>
    public virtual void ChangeName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        var oldName = Name;
        Name = name;

        AddLocalEvent(new IdentityRoleNameChangedEto
        {
            Name = name,
            Id = Id,
            TenantId = TenantId,
            OldName = oldName
        });

        AddDistributedEvent(new IdentityRoleNameChangedEto
        {
            Id = Id,
            Name = Name,
            OldName = oldName,
            TenantId = TenantId
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{base.ToString()}, Name = {Name}";
    }
}
