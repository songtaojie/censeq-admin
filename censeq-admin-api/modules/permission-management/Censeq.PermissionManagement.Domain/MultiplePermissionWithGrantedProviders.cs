using System.Collections.Generic;
using Volo.Abp;

namespace Censeq.PermissionManagement;

/// <summary>
/// 多个权限及其授予提供者信息。
/// </summary>
public class MultiplePermissionWithGrantedProviders
{
    /// <summary>
    /// 权限授权结果列表。
    /// </summary>
    public List<PermissionWithGrantedProviders> Result { get; }

    /// <summary>
    /// 初始化 MultiplePermissionWithGrantedProviders 实例。
    /// </summary>
    public MultiplePermissionWithGrantedProviders()
    {
        Result = [];
    }

    /// <summary>
    /// 初始化 MultiplePermissionWithGrantedProviders 实例。
    /// </summary>
    /// <param name="names">names。</param>
    public MultiplePermissionWithGrantedProviders(string[] names)
    {
        Check.NotNull(names, nameof(names));
        Result = new List<PermissionWithGrantedProviders>(names.Length);
        foreach (var name in names)
            Result.Add(new PermissionWithGrantedProviders(name, false));
    }
}
