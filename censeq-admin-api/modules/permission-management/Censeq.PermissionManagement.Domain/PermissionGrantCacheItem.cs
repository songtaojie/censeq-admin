using System;
using System.Linq;
using Volo.Abp.Text.Formatting;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予缓存项。
/// </summary>
[Serializable]
public class PermissionGrantCacheItem
{
    private const string CacheKeyFormat = "pn:{0},pk:{1},n:{2}";

    /// <summary>
    /// 是否已授予权限。
    /// </summary>
    public bool IsGranted { get; set; }

    /// <summary>
    /// 初始化 PermissionGrantCacheItem 实例。
    /// </summary>
    public PermissionGrantCacheItem() { }

    /// <summary>
    /// 初始化 PermissionGrantCacheItem 实例。
    /// </summary>
    /// <param name="isGranted">是否已授予权限。</param>
    public PermissionGrantCacheItem(bool isGranted)
    {
        IsGranted = isGranted;
    }

    /// <summary>
    /// 计算权限授予缓存键。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予缓存键。</returns>
    public static string CalculateCacheKey(string name, string providerName, string? providerKey)
    {
        return string.Format(CacheKeyFormat, providerName, providerKey, name);
    }

    /// <summary>
    /// 从权限授予缓存键中解析权限名称。
    /// </summary>
    /// <param name="cacheKey">权限授予缓存键。</param>
    /// <returns>解析出的权限名称；解析失败时返回 null。</returns>
    public static string? GetPermissionNameFormCacheKeyOrNull(string cacheKey)
    {
        var result = FormattedStringValueExtracter.Extract(cacheKey, CacheKeyFormat, true);
        return result.IsMatch ? result.Matches.Last().Value : null;
    }
}
