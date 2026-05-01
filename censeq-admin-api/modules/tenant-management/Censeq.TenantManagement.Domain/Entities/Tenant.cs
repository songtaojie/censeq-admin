using System;
using System.Collections.Generic;
using System.Linq;
using Censeq.TenantManagement;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Censeq.TenantManagement.Entities;

public class Tenant : FullAuditedAggregateRoot<Guid>, IHasEntityVersion
{
    public virtual string Name { get; protected set; }

    public virtual string NormalizedName { get; protected set; }

    public virtual string? Code { get; protected set; }

    public virtual string? Domain { get; protected set; }

    public virtual string? Icon { get; protected set; }

    public virtual string? Copyright { get; protected set; }

    public virtual string? IcpNo { get; protected set; }

    public virtual string? IcpAddress { get; protected set; }

    public virtual string? Remark { get; protected set; }

    public virtual int MaxUserCount { get; protected set; }

    public virtual bool IsActive { get; protected set; }

    public virtual int EntityVersion { get; protected set; }

    public virtual List<TenantConnectionString> ConnectionStrings { get; protected set; }

    protected Tenant()
    {

    }

    protected internal Tenant(Guid id, [NotNull] string name, [CanBeNull] string? normalizedName, [CanBeNull] string? code = null)
        : base(id)
    {
        SetName(name);
        SetNormalizedName(normalizedName);
        SetCode(code);
        MaxUserCount = 0;
        IsActive = true;

        ConnectionStrings = new List<TenantConnectionString>();
    }

    [CanBeNull]
    public virtual string? FindDefaultConnectionString()
    {
        return FindConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
    }

    [CanBeNull]
    public virtual string? FindConnectionString(string name)
    {
        return ConnectionStrings.FirstOrDefault(c => c.Name == name)?.Value;
    }

    public virtual void SetDefaultConnectionString(string connectionString)
    {
        SetConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName, connectionString);
    }

    public virtual void SetConnectionString(string name, string connectionString)
    {
        var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

        if (tenantConnectionString != null)
        {
            tenantConnectionString.SetValue(connectionString);
        }
        else
        {
            ConnectionStrings.Add(new TenantConnectionString(Id, name, connectionString));
        }
    }

    public virtual void RemoveDefaultConnectionString()
    {
        RemoveConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
    }

    public virtual void RemoveConnectionString(string name)
    {
        var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);

        if (tenantConnectionString != null)
        {
            ConnectionStrings.Remove(tenantConnectionString);
        }
    }

    protected internal virtual void SetName([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
    }

    protected internal virtual void SetNormalizedName([CanBeNull] string? normalizedName)
    {
        NormalizedName = normalizedName ?? Name;
    }

    /// <summary>
    /// 将登录/表单输入规范为持久化形式（去空白、统一大写），便于不区分大小写匹配。
    /// </summary>
    [CanBeNull]
    public static string? NormalizeCode([CanBeNull] string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return raw.Trim().ToUpperInvariant();
    }

    [CanBeNull]
    public static string? NormalizeDomain([CanBeNull] string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var value = raw.Trim();
        if (Uri.TryCreate(value, UriKind.Absolute, out var absoluteUri))
        {
            value = absoluteUri.Authority;
        }
        else
        {
            var pathStart = value.IndexOf('/');
            if (pathStart >= 0)
            {
                value = value[..pathStart];
            }
        }

        return value.Trim().TrimEnd('/').ToLowerInvariant();
    }

    protected internal virtual void SetCode([CanBeNull] string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Code = null;
            return;
        }

        Code = Check.Length(code, nameof(code), TenantConsts.MaxCodeLength);
    }

    public virtual void SetCodePublic([CanBeNull] string? code)
    {
        SetCode(code);
    }

    public virtual void SetDomain([CanBeNull] string? domain)
    {
        Domain = NormalizeOptional(NormalizeDomain(domain), TenantConsts.MaxDomainLength, nameof(domain));
    }

    public virtual void SetIcon([CanBeNull] string? icon)
    {
        Icon = NormalizeOptional(icon, TenantConsts.MaxIconLength, nameof(icon));
    }

    public virtual void SetCopyright([CanBeNull] string? copyright)
    {
        Copyright = NormalizeOptional(copyright, TenantConsts.MaxCopyrightLength, nameof(copyright));
    }

    public virtual void SetIcpNo([CanBeNull] string? icpNo)
    {
        IcpNo = NormalizeOptional(icpNo, TenantConsts.MaxIcpNoLength, nameof(icpNo));
    }

    public virtual void SetIcpAddress([CanBeNull] string? icpAddress)
    {
        IcpAddress = NormalizeOptional(icpAddress, TenantConsts.MaxIcpAddressLength, nameof(icpAddress));
    }

    public virtual void SetRemark([CanBeNull] string? remark)
    {
        Remark = NormalizeOptional(remark, TenantConsts.MaxRemarkLength, nameof(remark));
    }

    public virtual void SetMaxUserCount(int maxUserCount)
    {
        if (maxUserCount < 0)
        {
            throw new BusinessException("Censeq.TenantManagement:InvalidMaxUserCount");
        }

        MaxUserCount = maxUserCount;
    }

    public virtual void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    private static string? NormalizeOptional(string? value, int maxLength, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Check.Length(value.Trim(), propertyName, maxLength);
    }
}
