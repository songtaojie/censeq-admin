using System.Linq;
using System.Threading.Tasks;
using Censeq.PermissionManagement.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理提供者基类。
/// 每个提供者代表一种授权主体类型，例如角色、用户或客户端。
/// </summary>
public abstract class PermissionManagementProvider : IPermissionManagementProvider
{
    /// <summary>
    /// 权限提供者名称。
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// 权限授予仓储。
    /// </summary>
    protected IPermissionGrantRepository PermissionGrantRepository { get; }

    /// <summary>
    /// Guid 生成器。
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }

    /// <summary>
    /// 当前租户上下文。
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }

    /// <summary>
    /// 初始化权限管理提供者。
    /// </summary>
    /// <param name="permissionGrantRepository">权限授予仓储。</param>
    /// <param name="guidGenerator">Guid 生成器。</param>
    /// <param name="currentTenant">当前租户上下文。</param>
    protected PermissionManagementProvider(
        IPermissionGrantRepository permissionGrantRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        PermissionGrantRepository = permissionGrantRepository;
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
    }

    /// <summary>
    /// 检查单个权限是否已授予给指定提供者标识。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">请求检查的权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予结果。</returns>
    public virtual async Task<PermissionValueProviderGrantInfo> CheckAsync(string name, string providerName, string providerKey)
    {
        var multiple = await CheckAsync(new[] { name }, providerName, providerKey);
        return multiple.Result.First().Value;
    }

    /// <summary>
    /// 批量检查权限是否已授予给指定提供者标识。
    /// 当请求的提供者名称与当前提供者不一致时，直接返回未授予结果。
    /// </summary>
    /// <param name="names">权限名称集合。</param>
    /// <param name="providerName">请求检查的权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>批量权限授予结果。</returns>
    public virtual async Task<MultiplePermissionValueProviderGrantInfo> CheckAsync(string[] names, string providerName, string providerKey)
    {
        var result = new MultiplePermissionValueProviderGrantInfo(names);
        if (providerName != Name) return result;
        var permissionGrants = await PermissionGrantRepository.GetListAsync(names, providerName, providerKey);
        foreach (var permissionName in names)
        {
            var isGrant = permissionGrants.Any(x => x.Name == permissionName);
            result.Result[permissionName] = new PermissionValueProviderGrantInfo(isGrant, providerKey);
        }
        return result;
    }

    /// <summary>
    /// 设置权限授予状态。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="isGranted">是否授予。</param>
    /// <returns>异步任务。</returns>
    public virtual Task SetAsync(string name, string providerKey, bool isGranted)
    {
        return isGranted ? GrantAsync(name, providerKey) : RevokeAsync(name, providerKey);
    }

    /// <summary>
    /// 授予权限。
    /// 已存在相同授权记录时不会重复写入。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>异步任务。</returns>
    protected virtual async Task GrantAsync(string name, string providerKey)
    {
        var permissionGrant = await PermissionGrantRepository.FindAsync(name, Name, providerKey);
        if (permissionGrant != null) return;
        await PermissionGrantRepository.InsertAsync(new PermissionGrant(GuidGenerator.Create(), name, Name, providerKey, CurrentTenant.Id));
    }

    /// <summary>
    /// 撤销权限。
    /// 不存在授权记录时直接返回。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>异步任务。</returns>
    protected virtual async Task RevokeAsync(string name, string providerKey)
    {
        var permissionGrant = await PermissionGrantRepository.FindAsync(name, Name, providerKey);
        if (permissionGrant == null) return;
        await PermissionGrantRepository.DeleteAsync(permissionGrant);
    }
}
