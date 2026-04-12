using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Censeq.Admin.Menus;

public class MenuPermission : Entity<Guid>
{
    public virtual Guid MenuId { get; protected set; }

    public virtual string PermissionName { get; protected set; } = string.Empty;

    protected MenuPermission()
    {
    }

    public MenuPermission(Guid id, Guid menuId, [NotNull] string permissionName)
        : base(id)
    {
        MenuId = menuId;
        SetPermissionName(permissionName);
    }

    public virtual void SetPermissionName([NotNull] string permissionName)
    {
        PermissionName = Check.NotNullOrWhiteSpace(permissionName, nameof(permissionName), MenuConsts.MaxNameLength);
    }
}