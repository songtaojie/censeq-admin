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
    /// 为给定数字创建代码。
    /// 示例：如果数字是 4,2，则返回 "00004.00002"；
    /// </summary>
    /// <param name="numbers">数字</param>
    public static string CreateCode(params int[] numbers)
    {
        if (numbers.IsNullOrEmpty())
        {
            return null!;
        }

        return numbers.Select(number => number.ToString(new string('0', OrganizationUnitConsts.CodeUnitLength))).JoinAsString(".");
    }

    /// <summary>
    /// 将子代码追加到父代码。
    /// 示例：如果 parentCode = "00001"，childCode = "00042"，则返回 "00001.00042"。
    /// </summary>
    /// <param name="parentCode">父代码。如果父级是根节点，可以为 null 或空。</param>
    /// <param name="childCode">子代码。</param>
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
    /// 获取相对于父级的代码。
    /// 示例：如果 code = "00019.00055.00001" 且 parentCode = "00019"，则返回 "00055.00001"。
    /// </summary>
    /// <param name="code">代码。</param>
    /// <param name="parentCode">父级代码。</param>
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
    /// 计算给定代码的下一个代码。
    /// 示例：如果 code = "00019.00055.00001"，则返回 "00019.00055.00002"。
    /// </summary>
    /// <param name="code">代码。</param>
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
    /// 获取最后一个单元代码。
    /// 示例：如果 code = "00019.00055.00001"，则返回 "00001"。
    /// </summary>
    /// <param name="code">代码。</param>
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
    /// 获取父级代码。
    /// 示例：如果 code = "00019.00055.00001"，则返回 "00019.00055"。
    /// </summary>
    /// <param name="code">代码。</param>
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
