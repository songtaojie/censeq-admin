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

    protected internal virtual void SetNormalizedName([CanBeNull] string normalizedName)
    {
        NormalizedName = normalizedName;
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
}
