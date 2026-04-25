using System;
using System.Collections.Generic;

namespace Censeq.Identity;

/// <summary>
/// 身份安全日志上下文
/// </summary>
public class IdentitySecurityLogContext
{
    public string Identity { get; set; }

    public string Action { get; set; }

    public string UserName { get; set; }

    public string ClientId { get; set; }

    /// <summary>
    /// Dictionary<string, object>
    /// </summary>
    public Dictionary<string, object> ExtraProperties { get; }

    public IdentitySecurityLogContext()
    {
        ExtraProperties = new Dictionary<string, object>();
    }

    /// <summary>
    /// 身份安全日志上下文
    /// </summary>
    public virtual IdentitySecurityLogContext WithProperty(string key, object value)
    {
        ExtraProperties[key] = value;
        return this;
    }

}
