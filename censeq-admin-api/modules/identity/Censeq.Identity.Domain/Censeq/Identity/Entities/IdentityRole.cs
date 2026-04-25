using System;
using Censeq.Identity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using JetBrains.Annotations;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 身份系统中的角色
/// </summary>
public class IdentityRole : AggregateRoot<Guid>, IMultiTenant, IHasEntityVersion
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public virtual string Name { get; protected internal set; }

    /// <summary>
    /// 标准化角色名称
    /// </summary>
    [DisableAuditing]
    public virtual string NormalizedName { get; protected internal set; }

    /// <summary>
    /// 角色业务代码
    /// </summary>
    public virtual string? Code { get; protected internal set; }

    /// <summary>
    /// 角色声明导航属性
    /// </summary>
    public virtual ICollection<IdentityRoleClaim> Claims { get; protected set; }

    /// <summary>
    /// 默认角色会自动分配给新用户
    /// </summary>
    public virtual bool IsDefault { get; set; }

    /// <summary>
    /// 静态角色不能被删除或重命名
    /// </summary>
    public virtual bool IsStatic { get; set; }

    /// <summary>
    /// 用户可以看到其他用户的公开角色
    /// </summary>
    public virtual bool IsPublic { get; set; }

    /// <summary>
    /// 实体变更时递增的版本值
    /// </summary>
    public virtual int EntityVersion { get; protected set; }

    /// <summary>
    /// 初始化 <see cref="IdentityRole"/> 的新实例
    /// </summary>
    protected IdentityRole() { }

    public IdentityRole(Guid id, [NotNull] string name, Guid? tenantId = null)
    {
        Check.NotNull(name, nameof(name));

        Id = id;
        Name = name;
        TenantId = tenantId;
        NormalizedName = name.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");

        Claims = new Collection<IdentityRoleClaim>();
    }

    public virtual void AddClaim([NotNull] IGuidGenerator guidGenerator, [NotNull] Claim claim)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claim, nameof(claim));

        Claims.Add(new IdentityRoleClaim(guidGenerator.Create(), Id, claim, TenantId));
    }

    public virtual void AddClaims([NotNull] IGuidGenerator guidGenerator, [NotNull] IEnumerable<Claim> claims)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claims, nameof(claims));

        foreach (var claim in claims)
        {
            AddClaim(guidGenerator, claim);
        }
    }

    /// <summary>
    /// 身份角色声明
    /// </summary>
    public virtual IdentityRoleClaim FindClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));

        return Claims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value)!;
    }

    public virtual void RemoveClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));

        Claims.RemoveAll(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
    }

    public virtual void ChangeName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        var oldName = Name;
        Name = name;

        AddLocalEvent(
#pragma warning disable 618
                new IdentityRoleNameChangedEvent
#pragma warning restore 618
                {
                    IdentityRole = this,
                    OldName = oldName
                }
        );

        AddDistributedEvent(
            new IdentityRoleNameChangedEto
            {
                Id = Id,
                Name = Name,
                OldName = oldName,
                TenantId = TenantId
            }
        );
    }

    public virtual void SetCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            Code = null;
            return;
        }

        Code = Check.Length(code.Trim().ToUpperInvariant(), nameof(code), IdentityRoleConsts.MaxCodeLength);
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Name = {Name}";
    }
}
