using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Admin.Menus;

public class Menu : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid? ParentId { get; protected set; }

    public virtual string Name { get; protected set; } = string.Empty;

    public virtual string Title { get; protected set; } = string.Empty;

    public virtual string? RouteName { get; protected set; }

    public virtual string? Path { get; protected set; }

    public virtual string? Component { get; protected set; }

    public virtual string? Redirect { get; protected set; }

    public virtual string? Icon { get; protected set; }

    public virtual MenuType Type { get; protected set; }

    public virtual int Sort { get; protected set; }

    public virtual bool Visible { get; protected set; }

    public virtual bool KeepAlive { get; protected set; }

    public virtual bool Affix { get; protected set; }

    public virtual bool IsExternal { get; protected set; }

    public virtual string? ExternalUrl { get; protected set; }

    public virtual bool IsIframe { get; protected set; }

    public virtual bool Status { get; protected set; }

    public virtual MenuAuthorizationMode AuthorizationMode { get; protected set; }

    public virtual string? Remark { get; protected set; }

    public virtual string? ButtonCode { get; protected set; }

    public virtual string? PermissionGroups { get; protected set; }

    public virtual ICollection<MenuPermission> Permissions { get; protected set; }

    protected Menu()
    {
        Permissions = new List<MenuPermission>();
    }

    public Menu(Guid id, Guid? tenantId, [NotNull] string name, [NotNull] string title, MenuType type)
        : base(id)
    {
        TenantId = tenantId;
        Permissions = new List<MenuPermission>();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
        Visible = true;
        KeepAlive = true;
        Status = true;
        AuthorizationMode = MenuAuthorizationMode.Anonymous;

        SetName(name);
        SetTitle(title);
        SetType(type);
    }

    public virtual void SetParent(Guid? parentId)
    {
        ParentId = parentId;
    }

    public virtual void SetName([NotNull] string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), MenuConsts.MaxNameLength);
    }

    public virtual void SetTitle([NotNull] string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), MenuConsts.MaxTitleLength);
    }

    public virtual void SetRouteName([CanBeNull] string? routeName)
    {
        RouteName = NormalizeOptional(routeName, MenuConsts.MaxRouteNameLength, nameof(routeName));
    }

    public virtual void SetPath([CanBeNull] string? path)
    {
        Path = NormalizeOptional(path, MenuConsts.MaxPathLength, nameof(path));
    }

    public virtual void SetComponent([CanBeNull] string? component)
    {
        Component = NormalizeOptional(component, MenuConsts.MaxComponentLength, nameof(component));
    }

    public virtual void SetRedirect([CanBeNull] string? redirect)
    {
        Redirect = NormalizeOptional(redirect, MenuConsts.MaxPathLength, nameof(redirect));
    }

    public virtual void SetIcon([CanBeNull] string? icon)
    {
        Icon = NormalizeOptional(icon, MenuConsts.MaxIconLength, nameof(icon));
    }

    public virtual void SetType(MenuType type)
    {
        Type = type;

        if (type == MenuType.Button)
        {
            Path = null;
            Component = null;
            Redirect = null;
            KeepAlive = false;
            Affix = false;
            IsExternal = false;
            IsIframe = false;
            ExternalUrl = null;
        }
    }

    public virtual void SetSort(int sort)
    {
        Sort = sort;
    }

    public virtual void SetDisplayOptions(bool visible, bool keepAlive, bool affix)
    {
        Visible = visible;
        KeepAlive = Type == MenuType.Button ? false : keepAlive;
        Affix = Type == MenuType.Button ? false : affix;
    }

    public virtual void SetLinkOptions(bool isExternal, string? externalUrl, bool isIframe)
    {
        if (Type == MenuType.Button)
        {
            IsExternal = false;
            ExternalUrl = null;
            IsIframe = false;
            return;
        }

        IsExternal = isExternal;
        IsIframe = isIframe;
        ExternalUrl = (isExternal || isIframe)
            ? NormalizeOptional(externalUrl, MenuConsts.MaxExternalUrlLength, nameof(externalUrl))
            : null;
    }

    public virtual void SetStatus(bool status)
    {
        Status = status;
    }

    public virtual void SetAuthorizationMode(MenuAuthorizationMode authorizationMode)
    {
        AuthorizationMode = authorizationMode;
    }

    public virtual void SetRemark(string? remark)
    {
        Remark = NormalizeOptional(remark, MenuConsts.MaxRemarkLength, nameof(remark));
    }

    public virtual void SetButtonCode(string? buttonCode)
    {
        ButtonCode = Type == MenuType.Button
            ? NormalizeOptional(buttonCode, MenuConsts.MaxButtonCodeLength, nameof(buttonCode))
            : null;
    }

    public virtual void SetPermissionGroups(string? permissionGroups)
    {
        PermissionGroups = NormalizeOptional(permissionGroups, MenuConsts.MaxPermissionGroupsLength, nameof(permissionGroups));
    }

    public virtual void SetPermissions(IEnumerable<MenuPermission> permissions)
    {
        Permissions = permissions.ToList();
    }

    private static string? NormalizeOptional(string? value, int maxLength, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Check.Length(value.Trim(), propertyName, maxLength);
    }
}