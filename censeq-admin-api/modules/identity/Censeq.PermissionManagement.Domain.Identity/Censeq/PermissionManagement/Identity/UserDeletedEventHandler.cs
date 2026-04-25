using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Censeq.PermissionManagement.Identity;

/// <summary>
/// 用户Deleted事件处理器
/// </summary>
public class UserDeletedEventHandler :
    IDistributedEventHandler<EntityDeletedEto<UserEto>>,
    ITransientDependency
{
    /// <summary>
    /// I权限管理器
    /// </summary>
    protected IPermissionManager PermissionManager { get; }

    public UserDeletedEventHandler(IPermissionManager permissionManager)
    {
        PermissionManager = permissionManager;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(EntityDeletedEto<UserEto> eventData)
    {
        await PermissionManager.DeleteAsync(UserPermissionValueProvider.ProviderName, eventData.Entity.Id.ToString());
    }
}
