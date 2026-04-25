using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 组织单元（OU）
/// </summary>
public class OrganizationUnit : FullAuditedAggregateRoot<Guid>, IMultiTenant, IHasEntityVersion
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 父级组织单元标识，如果为根节点则为 null
    /// </summary>
    public virtual Guid? ParentId { get; internal set; }

    /// <summary>
    /// 组织单元的层级代码。
    /// 示例："00001.00042.00005"。
    /// 这是组织单元的唯一代码。
    /// 如果组织单元层级发生变化，代码也会改变。
    /// </summary>
    public virtual string Code { get; internal set; }

    /// <summary>
    /// 组织单元显示名称
    /// </summary>
    public virtual string DisplayName { get; set; }

    /// <summary>
    /// 状态：1=启用 0=禁用
    /// </summary>
    public virtual int Status { get; set; } = 1;

    /// <summary>
    /// 备注/描述
    /// </summary>
    public virtual string? Remark { get; set; }

    /// <summary>
    /// 实体变更时递增的版本值
    /// </summary>
    public virtual int EntityVersion { get; set; }

    /// <summary>
    /// 组织单元的角色
    /// </summary>
    public virtual ICollection<OrganizationUnitRole> Roles { get; protected set; }

    /// <summary>
    /// 初始化 <see cref="OrganizationUnit"/> 类的新实例
    /// </summary>
    public OrganizationUnit()
    {

    }

    /// <summary>
    /// 初始化 <see cref="OrganizationUnit"/> 类的新实例
    /// </summary>
    /// <param name="id">标识</param>
    /// <param name="displayName">显示名称</param>
    /// <param name="parentId">父级标识，如果为根节点则为 null</param>
    /// <param name="tenantId">租户标识，宿主端为 null</param>
    public OrganizationUnit(Guid id, string displayName, Guid? parentId = null, Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        DisplayName = displayName;
        ParentId = parentId;
        Roles = new Collection<OrganizationUnitRole>();
    }

    /// <summary>
    /// Creates code for given numbers.
    /// Example: if numbers are 4,2 then returns "00004.00002";
    /// </summary>
    /// <param name="numbers">Numbers</param>
    public static string CreateCode(params int[] numbers)
    {
        if (numbers.IsNullOrEmpty())
        {
            return null!;
        }

        return numbers.Select(number => number.ToString(new string('0', OrganizationUnitConsts.CodeUnitLength))).JoinAsString(".");
    }

    /// <summary>
    /// Appends a child code to a parent code.
    /// Example: if parentCode = "00001", childCode = "00042" then returns "00001.00042".
    /// </summary>
    /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
    /// <param name="childCode">Child code.</param>
    public static string AppendCode(string parentCode, string childCode)
    {
        if (childCode.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
        }

        if (parentCode.IsNullOrEmpty())
        {
            return childCode;
        }

        return parentCode + "." + childCode;
    }

    /// <summary>
    /// Gets relative code to the parent.
    /// Example: if code = "00019.00055.00001" and parentCode = "00019" then returns "00055.00001".
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="parentCode">The parent code.</param>
    public static string GetRelativeCode(string code, string parentCode)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        if (parentCode.IsNullOrEmpty())
        {
            return code;
        }

        if (code.Length == parentCode.Length)
        {
            return null!;
        }

        return code.Substring(parentCode.Length + 1);
    }

    /// <summary>
    /// Calculates next code for given code.
    /// Example: if code = "00019.00055.00001" returns "00019.00055.00002".
    /// </summary>
    /// <param name="code">The code.</param>
    public static string CalculateNextCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var parentCode = GetParentCode(code);
        var lastUnitCode = GetLastUnitCode(code);

        return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
    }

    /// <summary>
    /// Gets the last unit code.
    /// Example: if code = "00019.00055.00001" returns "00001".
    /// </summary>
    /// <param name="code">The code.</param>
    public static string GetLastUnitCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var splittedCode = code.Split('.');
        return splittedCode[splittedCode.Length - 1];
    }

    /// <summary>
    /// Gets parent code.
    /// Example: if code = "00019.00055.00001" returns "00019.00055".
    /// </summary>
    /// <param name="code">The code.</param>
    public static string GetParentCode(string code)
    {
        if (code.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
        }

        var splittedCode = code.Split('.');
        if (splittedCode.Length == 1)
        {
            return null!;
        }

        return splittedCode.Take(splittedCode.Length - 1).JoinAsString(".");
    }

    public virtual void AddRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));

        if (IsInRole(roleId))
        {
            return;
        }

        Roles.Add(new OrganizationUnitRole(roleId, Id, TenantId));
    }

    public virtual void RemoveRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));

        if (!IsInRole(roleId))
        {
            return;
        }

        Roles.RemoveAll(r => r.RoleId == roleId);
    }

    public virtual bool IsInRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));

        return Roles.Any(r => r.RoleId == roleId);
    }
}
