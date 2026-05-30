using Censeq.PermissionManagement.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SimpleStateChecking;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理器。
/// 统一校验权限定义、租户侧和提供者范围，再调用对应的权限管理提供者读写授权。
/// </summary>
public class PermissionManager : IPermissionManager, ISingletonDependency
{
    /// <summary>
    /// 授权仓储
    /// </summary>
    protected IPermissionGrantRepository PermissionGrantRepository { get; }

    /// <summary>
    /// 权限定义管理器
    /// </summary>
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }

    /// <summary>
    /// 状态检查
    /// </summary>
    protected ISimpleStateCheckerManager<PermissionDefinition> SimpleStateCheckerManager { get; }

    /// <summary>
    /// Guid生成器
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }

    /// <summary>
    /// 当前租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }

    /// <summary>
    /// 已注册的权限管理提供者。
    /// 通过配置延迟解析，避免在管理器初始化时提前创建所有提供者实例。
    /// </summary>
    protected IReadOnlyList<IPermissionManagementProvider> ManagementProviders => _lazyProviders.Value;

    /// <summary>
    /// 权限配置
    /// </summary>
    protected PermissionManagementOptions Options { get; }

    /// <summary>
    /// 分布式缓存
    /// </summary>
    protected IDistributedCache<PermissionGrantCacheItem> Cache { get; }

    private readonly Lazy<List<IPermissionManagementProvider>> _lazyProviders;

    /// <summary>
    /// 初始化权限管理器。
    /// </summary>
    /// <param name="permissionDefinitionManager">权限定义管理器。</param>
    /// <param name="simpleStateCheckerManager">权限状态检查器。</param>
    /// <param name="permissionGrantRepository">权限授予仓储。</param>
    /// <param name="serviceProvider">服务提供器，用于延迟解析权限提供者。</param>
    /// <param name="guidGenerator">Guid 生成器。</param>
    /// <param name="options">权限管理配置。</param>
    /// <param name="currentTenant">当前租户上下文。</param>
    /// <param name="cache">权限授予缓存。</param>
    public PermissionManager(
        IPermissionDefinitionManager permissionDefinitionManager,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager,
        IPermissionGrantRepository permissionGrantRepository,
        IServiceProvider serviceProvider,
        IGuidGenerator guidGenerator,
        IOptions<PermissionManagementOptions> options,
        ICurrentTenant currentTenant,
        IDistributedCache<PermissionGrantCacheItem> cache)
    {
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
        Cache = cache;
        SimpleStateCheckerManager = simpleStateCheckerManager;
        PermissionGrantRepository = permissionGrantRepository;
        PermissionDefinitionManager = permissionDefinitionManager;
        Options = options.Value;

        _lazyProviders = new Lazy<List<IPermissionManagementProvider>>(
            () => Options
                .ManagementProviders
                .Select(c => (serviceProvider.GetRequiredService(c) as IPermissionManagementProvider)!)
                .ToList(),
            true
        );
    }

    /// <summary>
    /// 获取单个权限在指定提供者上的授予情况。
    /// </summary>
    /// <param name="permissionName">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限是否已授予，以及实际授予来源。</returns>
    public virtual async Task<PermissionWithGrantedProviders> GetAsync(string permissionName, string providerName, string providerKey)
    {
        var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
        if (permission == null)
        {
            return new PermissionWithGrantedProviders(permissionName, false);
        }

        return await GetInternalAsync(
            permission,
            providerName,
            providerKey
        );
    }

    /// <summary>
    /// 批量获取权限在指定提供者上的授予情况。
    /// </summary>
    /// <param name="permissionNames">权限名称集合。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>多个权限的授予状态。</returns>
    public virtual async Task<MultiplePermissionWithGrantedProviders> GetAsync(string[] permissionNames, string providerName, string providerKey)
    {
        var permissions = new List<PermissionDefinition>();
        var undefinedPermissions = new List<string>();

        foreach (var permissionName in permissionNames)
        {
            var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
            if (permission != null)
            {
                permissions.Add(permission);
            }
            else
            {
                undefinedPermissions.Add(permissionName);
            }
        }

        if (permissions.Count == 0)
        {
            return new MultiplePermissionWithGrantedProviders(undefinedPermissions.ToArray());
        }

        var result = await GetInternalAsync(
            [.. permissions],
            providerName,
            providerKey
        );

        foreach (var undefinedPermission in undefinedPermissions)
        {
            result.Result.Add(new PermissionWithGrantedProviders(undefinedPermission, false));
        }

        return result;
    }

    /// <summary>
    /// 获取指定提供者上的全部权限授予情况。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>全部权限的授予状态列表。</returns>
    public virtual async Task<List<PermissionWithGrantedProviders>> GetAllAsync(string providerName, string providerKey)
    {
        var permissionDefinitions = (await PermissionDefinitionManager.GetPermissionsAsync()).ToArray();

        var multiplePermissionWithGrantedProviders = await GetInternalAsync(permissionDefinitions, providerName, providerKey);

        return multiplePermissionWithGrantedProviders.Result;

    }

    /// <summary>
    /// 设置单个权限的授予状态。
    /// 设置前会验证权限是否存在、是否启用、是否支持当前提供者和当前租户侧。
    /// </summary>
    /// <param name="permissionName">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="isGranted">是否授予。</param>
    /// <returns>异步任务。</returns>
    /// <exception cref="BusinessException">权限不可管理或提供者不匹配时抛出。</exception>
    public virtual async Task SetAsync(string permissionName, string providerName, string providerKey, bool isGranted)
    {
        var permission = await PermissionDefinitionManager.GetOrNullAsync(permissionName);
        if (permission == null)
        {
            /* 默默忽略未定义的权限，也许它们已从动态权限定义存储中删除 */
            return;
        }

        if (!permission.IsEnabled || !await SimpleStateCheckerManager.IsEnabledAsync(permission))
        {
            throw new BusinessException($"权限[{permission.Name}]已被禁用!");
        }

        if (permission.Providers.Any() && !permission.Providers.Contains(providerName))
        {
            throw new BusinessException($"提供程序[{providerName}] 未定义权限[{permission.Name}]");
        }

        if (!permission.MultiTenancySide.HasFlag(CurrentTenant.GetMultiTenancySide()))
        {
            throw new BusinessException($"权限[{permission.Name}]具有多租户端[{permission.MultiTenancySide}]，它与当前多租户端[{CurrentTenant.GetMultiTenancySide()}]不兼容");
        }

        var currentGrantInfo = await GetInternalAsync(permission, providerName, providerKey);
        if (currentGrantInfo.IsGranted == isGranted)
        {
            return;
        }

        var provider = ManagementProviders.FirstOrDefault(m => m.Name == providerName) ?? throw new BusinessException("未知的权限管理提供商：" + providerName);
        await provider.SetAsync(permissionName, providerKey, isGranted);
    }

    /// <summary>
    /// 更新权限授予记录的提供者标识。
    /// 用于角色、用户等业务主键变更时同步授权数据。
    /// </summary>
    /// <param name="permissionGrant">待更新的权限授予记录。</param>
    /// <param name="providerKey">新的提供者标识。</param>
    /// <returns>更新后的权限授予记录。</returns>
    public virtual async Task<PermissionGrant> UpdateProviderKeyAsync(PermissionGrant permissionGrant, string providerKey)
    {
        using (CurrentTenant.Change(permissionGrant.TenantId))
        {
            //使旧密钥的缓存无效
            await Cache.RemoveAsync(
                PermissionGrantCacheItem.CalculateCacheKey(
                    permissionGrant.Name,
                    permissionGrant.ProviderName,
                    permissionGrant.ProviderKey
                )
            );
        }

        permissionGrant.ProviderKey = providerKey;
        return await PermissionGrantRepository.UpdateAsync(permissionGrant);
    }

    /// <summary>
    /// 删除指定提供者标识下的全部权限授予记录。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>异步任务。</returns>
    public virtual async Task DeleteAsync(string providerName, string providerKey)
    {
        var permissionGrants = await PermissionGrantRepository.GetListAsync(providerName, providerKey);
        foreach (var permissionGrant in permissionGrants)
        {
            await PermissionGrantRepository.DeleteAsync(permissionGrant);
        }
    }

    /// <summary>
    /// 获取单个权限的内部授予结果。
    /// </summary>
    /// <param name="permission">权限定义。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予结果。</returns>
    protected virtual async Task<PermissionWithGrantedProviders> GetInternalAsync(
        PermissionDefinition permission,
        string providerName,
        string providerKey)
    {
        var multiplePermissionWithGrantedProviders = await GetInternalAsync(
            [permission],
            providerName,
            providerKey
        );

        return multiplePermissionWithGrantedProviders.Result.First();
    }

    /// <summary>
    /// 对通过启用状态、租户侧和提供者范围校验的权限执行批量检查。
    /// </summary>
    /// <param name="permissions">权限定义集合。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>多个权限的内部授予结果。</returns>
    protected virtual async Task<MultiplePermissionWithGrantedProviders> GetInternalAsync(
        PermissionDefinition[] permissions,
        string providerName,
        string providerKey)
    {
        var permissionNames = permissions.Select(x => x.Name).ToArray();
        var multiplePermissionWithGrantedProviders = new MultiplePermissionWithGrantedProviders(permissionNames);

        var neededCheckPermissions = new List<PermissionDefinition>();

        foreach (var permission in permissions
                                    .Where(x => x.IsEnabled)
                                    .Where(x => x.MultiTenancySide.HasFlag(CurrentTenant.GetMultiTenancySide()))
                                    .Where(x => x.Providers.Count == 0 || x.Providers.Contains(providerName)))
        {
            if (await SimpleStateCheckerManager.IsEnabledAsync(permission))
            {
                neededCheckPermissions.Add(permission);
            }
        }

        if (neededCheckPermissions.Count == 0)
        {
            return multiplePermissionWithGrantedProviders;
        }

        foreach (var provider in ManagementProviders)
        {
            permissionNames = neededCheckPermissions.Select(x => x.Name).ToArray();
            var multiplePermissionValueProviderGrantInfo = await provider.CheckAsync(permissionNames, providerName, providerKey);

            foreach (var providerResultDict in multiplePermissionValueProviderGrantInfo.Result)
            {
                if (providerResultDict.Value.IsGranted)
                {
                    var permissionWithGrantedProvider = multiplePermissionWithGrantedProviders.Result
                        .First(x => x.Name == providerResultDict.Key);

                    permissionWithGrantedProvider.IsGranted = true;
                    permissionWithGrantedProvider.Providers.Add(new PermissionValueProviderInfo(provider.Name, providerResultDict.Value.ProviderKey!));
                }
            }
        }

        return multiplePermissionWithGrantedProviders;
    }
}
