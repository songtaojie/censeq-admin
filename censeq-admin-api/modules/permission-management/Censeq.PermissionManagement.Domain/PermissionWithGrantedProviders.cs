using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限及其授予提供者信息。
/// </summary>
public class PermissionWithGrantedProviders
{
    /// <summary>
    /// 权限名称。
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 是否已授予权限。
    /// </summary>
    public bool IsGranted { get; set; }
    /// <summary>
    /// 授予该权限的权限值提供者列表。
    /// </summary>
    public List<PermissionValueProviderInfo> Providers { get; set; }

    /// <summary>
    /// 初始化 PermissionWithGrantedProviders 实例。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="isGranted">是否已授予权限。</param>
    public PermissionWithGrantedProviders([NotNull] string name, bool isGranted)
    {
        Check.NotNull(name, nameof(name));
        Name = name;
        IsGranted = isGranted;
        Providers = new List<PermissionValueProviderInfo>();
    }
}
