using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement.OpenIddict;

/// <summary>
/// 应用程序权限Management提供者。
/// </summary>
public class ApplicationPermissionManagementProvider : PermissionManagementProvider
{
    /// <summary>
    /// 名称。
    /// </summary>
    public override string Name => ClientPermissionValueProvider.ProviderName;

    /// <summary>
    /// 初始化 ApplicationPermissionManagementProvider 实例。
    /// </summary>
    /// <param name="permissionGrantRepository">权限授权类型仓储。</param>
    /// <param name="guidGenerator">GUID生成器。</param>
    /// <param name="currentTenant">当前租户。</param>
    public ApplicationPermissionManagementProvider(
        IPermissionGrantRepository permissionGrantRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
        : base(
            permissionGrantRepository,
            guidGenerator,
            currentTenant)
    {

    }

    /// <summary>
    /// 检查权限授予状态。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="providerName">提供者Name。</param>
    /// <param name="providerKey">提供者Key。</param>
    /// <returns>异步操作结果。</returns>
    public override Task<PermissionValueProviderGrantInfo> CheckAsync(string name, string providerName, string providerKey)
    {
        using (CurrentTenant.Change(null))
        {
            return base.CheckAsync(name, providerName, providerKey);
        }
    }

    /// <summary>
    /// 授予权限。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="providerKey">提供者Key。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected override Task GrantAsync(string name, string providerKey)
    {
        using (CurrentTenant.Change(null))
        {
            return base.GrantAsync(name, providerKey);
        }
    }

    /// <summary>
    /// 撤销数据。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="providerKey">提供者Key。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected override Task RevokeAsync(string name, string providerKey)
    {
        using (CurrentTenant.Change(null))
        {
            return base.RevokeAsync(name, providerKey);
        }
    }

    /// <summary>
    /// 设置权限授予状态。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="providerKey">提供者Key。</param>
    /// <param name="isGranted">sGranted。</param>
    /// <returns>表示异步操作的任务。</returns>
    public override Task SetAsync(string name, string providerKey, bool isGranted)
    {
        using (CurrentTenant.Change(null))
        {
            return base.SetAsync(name, providerKey, isGranted);
        }
    }
}
