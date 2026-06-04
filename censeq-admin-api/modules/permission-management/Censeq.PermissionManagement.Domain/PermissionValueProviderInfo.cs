using JetBrains.Annotations;
using Volo.Abp;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限值提供者信息。
/// </summary>
public class PermissionValueProviderInfo
{
    /// <summary>
    /// 权限值提供者名称。
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 权限值提供者标识。
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 初始化 PermissionValueProviderInfo 实例。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="key">提供者标识。</param>
    public PermissionValueProviderInfo([NotNull] string name, [NotNull] string key)
    {
        Check.NotNull(name, nameof(name));
        Check.NotNull(key, nameof(key));
        Name = name;
        Key = key;
    }
}
