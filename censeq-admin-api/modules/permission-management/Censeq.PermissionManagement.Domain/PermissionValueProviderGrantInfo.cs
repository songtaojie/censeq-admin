namespace Censeq.PermissionManagement;

/// <summary>
/// 权限值提供者授予信息。
/// </summary>
public class PermissionValueProviderGrantInfo
{
    /// <summary>
    /// 未授予权限的默认结果。
    /// </summary>
    public static PermissionValueProviderGrantInfo NonGranted { get; } = new PermissionValueProviderGrantInfo(false);
    /// <summary>
    /// 是否已授予权限。
    /// </summary>
    public virtual bool IsGranted { get; }
    /// <summary>
    /// 权限提供者标识。
    /// </summary>
    public virtual string? ProviderKey { get; }

    /// <summary>
    /// 初始化 PermissionValueProviderGrantInfo 实例。
    /// </summary>
    /// <param name="isGranted">是否已授予权限。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    public PermissionValueProviderGrantInfo(bool isGranted, string? providerKey = null)
    {
        IsGranted = isGranted;
        ProviderKey = providerKey;
    }
}
