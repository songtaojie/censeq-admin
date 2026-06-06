using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限种子数据贡献者。
/// 默认把当前租户侧可用的角色权限授予给 admin 角色。
/// </summary>
public class PermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    /// <summary>
    /// 当前租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }
    /// <summary>
    /// 权限定义管理器
    /// </summary>
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }

    /// <summary>
    /// 权限种子数据
    /// </summary>
    protected IPermissionDataSeeder PermissionDataSeeder { get; }

    /// <summary>
    /// 初始化权限种子数据贡献者。
    /// </summary>
    /// <param name="permissionDefinitionManager">权限定义管理器。</param>
    /// <param name="permissionDataSeeder">权限种子数据服务。</param>
    /// <param name="currentTenant">当前租户上下文。</param>
    public PermissionDataSeedContributor(
        IPermissionDefinitionManager permissionDefinitionManager,
        IPermissionDataSeeder permissionDataSeeder,
        ICurrentTenant currentTenant)
    {
        PermissionDefinitionManager = permissionDefinitionManager;
        PermissionDataSeeder = permissionDataSeeder;
        CurrentTenant = currentTenant;
    }

    /// <summary>
    /// 写入权限种子数据。
    /// </summary>
    /// <param name="context">数据种子上下文。</param>
    /// <returns>异步任务。</returns>
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        var multiTenancySide = CurrentTenant.GetMultiTenancySide();
        var permissionNames = (await PermissionDefinitionManager.GetPermissionsAsync())
            .Where(p => p.MultiTenancySide.HasFlag(multiTenancySide))
            .Where(p => p.Providers.Count == 0 || p.Providers.Contains(RolePermissionValueProvider.ProviderName))
            .Select(p => p.Name)
            .ToArray();

        await PermissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            "管理员",
            permissionNames,
            context?.TenantId
        );
    }
}
