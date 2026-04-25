using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Censeq.Identity;

namespace Censeq.PermissionManagement.Identity;

/// <summary>
/// 角色更新事件处理器
/// </summary>
public class RoleUpdateEventHandler :
    IDistributedEventHandler<IdentityRoleNameChangedEto>,
    ITransientDependency
{
    /// <summary>
    /// I权限管理器
    /// </summary>
    protected IPermissionManager PermissionManager { get; }
    /// <summary>
    /// I权限Grant仓储
    /// </summary>
    protected IPermissionGrantRepository PermissionGrantRepository { get; }

    public RoleUpdateEventHandler(
        IPermissionManager permissionManager,
        IPermissionGrantRepository permissionGrantRepository)
    {
        PermissionManager = permissionManager;
        PermissionGrantRepository = permissionGrantRepository;
    }

    public async Task HandleEventAsync(IdentityRoleNameChangedEto eventData)
    {
        var permissionGrantsInRole = await PermissionGrantRepository.GetListAsync(RolePermissionValueProvider.ProviderName, eventData.OldName);
        foreach (var permissionGrant in permissionGrantsInRole)
        {
            await PermissionManager.UpdateProviderKeyAsync(permissionGrant, eventData.Name);
        }
    }
}
