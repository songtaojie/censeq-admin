using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Admin.Menus;

public class CreateMenuDto
{
    public Guid? ParentId { get; set; }

    [Required]
    [StringLength(MenuConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(MenuConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [StringLength(MenuConsts.MaxRouteNameLength)]
    public string? RouteName { get; set; }

    [StringLength(MenuConsts.MaxPathLength)]
    public string? Path { get; set; }

    [StringLength(MenuConsts.MaxComponentLength)]
    public string? Component { get; set; }

    [StringLength(MenuConsts.MaxPathLength)]
    public string? Redirect { get; set; }

    [StringLength(MenuConsts.MaxIconLength)]
    public string? Icon { get; set; }

    public MenuType Type { get; set; }

    public int Sort { get; set; }

    public bool Visible { get; set; } = true;

    public bool KeepAlive { get; set; } = true;

    public bool Affix { get; set; }

    public bool IsExternal { get; set; }

    [StringLength(MenuConsts.MaxExternalUrlLength)]
    public string? ExternalUrl { get; set; }

    public bool IsIframe { get; set; }

    public bool Status { get; set; } = true;

    public MenuAuthorizationMode AuthorizationMode { get; set; } = MenuAuthorizationMode.Anonymous;

    [StringLength(MenuConsts.MaxRemarkLength)]
    public string? Remark { get; set; }

    [StringLength(MenuConsts.MaxButtonCodeLength)]
    public string? ButtonCode { get; set; }

    public List<string> PermissionNames { get; set; } = [];
}

public class UpdateMenuDto : CreateMenuDto, IHasConcurrencyStamp
{
    public string ConcurrencyStamp { get; set; } = string.Empty;
}

public class SetMenuStatusDto
{
    public bool Status { get; set; }
}

public class MenuSortDto
{
    public Guid? ParentId { get; set; }

    public int Sort { get; set; }
}

public class CopyTenantMenusDto
{
    public bool ClearExisting { get; set; }
}

public class MenuPermissionGroupDto
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public List<MenuPermissionDefinitionDto> Permissions { get; set; } = [];
}

public class MenuPermissionDefinitionDto
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? ParentName { get; set; }
}

public class MenuTreeItemDto : EntityDto<Guid>
{
    public Guid? ParentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? RouteName { get; set; }

    public string? Path { get; set; }

    public string? Component { get; set; }

    public string? Icon { get; set; }

    public MenuType Type { get; set; }

    public int Sort { get; set; }

    public bool Visible { get; set; }

    public bool Status { get; set; }

    public MenuAuthorizationMode AuthorizationMode { get; set; }

    public string? ButtonCode { get; set; }

    public List<string> PermissionNames { get; set; } = [];

    public List<MenuTreeItemDto> Children { get; set; } = [];
}

public class MenuDetailDto : MenuTreeItemDto, IMultiTenant, IHasConcurrencyStamp
{
    public Guid? TenantId { get; set; }

    public string? Redirect { get; set; }

    public bool KeepAlive { get; set; }

    public bool Affix { get; set; }

    public bool IsExternal { get; set; }

    public string? ExternalUrl { get; set; }

    public bool IsIframe { get; set; }

    public string? Remark { get; set; }

    public string ConcurrencyStamp { get; set; } = string.Empty;
}

public class CurrentUserMenuResultDto
{
    public List<MenuRouteDto> Routes { get; set; } = [];

    public List<string> AuthBtnList { get; set; } = [];

    public string[] Roles { get; set; } = [];
}

public class MenuRouteDto
{
    public string Path { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Redirect { get; set; }

    public string? Component { get; set; }

    public MenuRouteMetaDto Meta { get; set; } = new();

    public List<MenuRouteDto> Children { get; set; } = [];
}

public class MenuRouteMetaDto
{
    public string Title { get; set; } = string.Empty;

    public string IsLink { get; set; } = string.Empty;

    public bool IsHide { get; set; }

    public bool IsKeepAlive { get; set; } = true;

    public bool IsAffix { get; set; }

    public bool IsIframe { get; set; }

    public List<string> Roles { get; set; } = [];

    public string Icon { get; set; } = string.Empty;
}