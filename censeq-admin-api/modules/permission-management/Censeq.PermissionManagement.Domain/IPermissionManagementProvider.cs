using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理提供者。
/// 负责某一种授权主体的权限检查和写入。
/// </summary>
public interface IPermissionManagementProvider : ISingletonDependency //TODO: Consider to remove this pre-assumption
{
    /// <summary>
    /// 提供者名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 检查单个权限是否已授予。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">请求检查的权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予结果。</returns>
    Task<PermissionValueProviderGrantInfo> CheckAsync([NotNull] string name, [NotNull] string providerName,[NotNull] string providerKey);

    /// <summary>
    /// 批量检查权限是否已授予。
    /// </summary>
    /// <param name="names">权限名称集合。</param>
    /// <param name="providerName">请求检查的权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>批量权限授予结果。</returns>
    Task<MultiplePermissionValueProviderGrantInfo> CheckAsync([NotNull] string[] names, [NotNull] string providerName,[NotNull] string providerKey);

    /// <summary>
    /// 设置权限授予状态。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="isGranted">是否授予。</param>
    /// <returns>异步任务。</returns>
    Task SetAsync([NotNull] string name,[NotNull] string providerKey, bool isGranted);
}
