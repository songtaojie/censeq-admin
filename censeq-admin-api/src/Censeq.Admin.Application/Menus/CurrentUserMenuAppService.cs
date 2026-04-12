using System.Linq;
using Censeq.Identity;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Admin.Menus;

[Authorize]
public class CurrentUserMenuAppService : AdminAppService, ICurrentUserMenuAppService
{
    private readonly IRepository<Menu, Guid> _menuRepository;
    private readonly IRepository<MenuPermission, Guid> _menuPermissionRepository;
    private readonly IUserRoleFinder _userRoleFinder;
    private readonly IPermissionChecker _permissionChecker;

    public CurrentUserMenuAppService(
        IRepository<Menu, Guid> menuRepository,
        IRepository<MenuPermission, Guid> menuPermissionRepository,
        IUserRoleFinder userRoleFinder,
        IPermissionChecker permissionChecker)
    {
        _menuRepository = menuRepository;
        _menuPermissionRepository = menuPermissionRepository;
        _userRoleFinder = userRoleFinder;
        _permissionChecker = permissionChecker;
    }

    public virtual async Task<CurrentUserMenuResultDto> GetAsync()
    {
        if (!CurrentUser.IsAuthenticated)
        {
            return new CurrentUserMenuResultDto();
        }

        var menus = await GetRuntimeMenusAsync();
        var permissionLookup = await GetPermissionLookupAsync(menus.Select(x => x.Id).ToList());
        var grantedPermissionLookup = await GetGrantedPermissionLookupAsync(permissionLookup.Values.SelectMany(x => x).Distinct().ToList());
        var authBtnList = new List<string>();
        var routes = BuildRoutes(null, menus, permissionLookup, grantedPermissionLookup, authBtnList);
        var roles = CurrentUser.Id.HasValue
            ? await _userRoleFinder.GetRoleNamesAsync(CurrentUser.Id.Value)
            : [];

        return new CurrentUserMenuResultDto
        {
            Routes = routes,
            AuthBtnList = authBtnList.Distinct(StringComparer.Ordinal).ToList(),
            Roles = roles
        };
    }

    private async Task<List<Menu>> GetRuntimeMenusAsync()
    {
        var currentTenantMenus = await GetMenusByTenantAsync(CurrentTenant.Id);
        if (CurrentTenant.Id.HasValue && currentTenantMenus.Count == 0)
        {
            return await GetMenusByTenantAsync(null);
        }

        return currentTenantMenus;
    }

    private async Task<List<Menu>> GetMenusByTenantAsync(Guid? tenantId)
    {
        var queryable = await _menuRepository.GetQueryableAsync();
        return await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.TenantId == tenantId && x.Status)
                .OrderBy(x => x.Sort)
                .ThenBy(x => x.CreationTime));
    }

    private async Task<Dictionary<Guid, List<string>>> GetPermissionLookupAsync(List<Guid> menuIds)
    {
        if (menuIds.Count == 0)
        {
            return new Dictionary<Guid, List<string>>();
        }

        var queryable = await _menuPermissionRepository.GetQueryableAsync();
        var permissions = await AsyncExecuter.ToListAsync(queryable.Where(x => menuIds.Contains(x.MenuId)));
        return permissions
            .GroupBy(x => x.MenuId)
            .ToDictionary(x => x.Key, x => x.Select(y => y.PermissionName).OrderBy(y => y).ToList());
    }

    private async Task<Dictionary<string, bool>> GetGrantedPermissionLookupAsync(List<string> permissionNames)
    {
        var result = new Dictionary<string, bool>(StringComparer.Ordinal);
        foreach (var permissionName in permissionNames)
        {
            result[permissionName] = await _permissionChecker.IsGrantedAsync(permissionName);
        }

        return result;
    }

    private List<MenuRouteDto> BuildRoutes(
        Guid? parentId,
        List<Menu> menus,
        Dictionary<Guid, List<string>> permissionLookup,
        Dictionary<string, bool> grantedPermissionLookup,
        List<string> authBtnList)
    {
        var routes = new List<MenuRouteDto>();
        var children = menus.Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreationTime)
            .ToList();

        foreach (var child in children)
        {
            var permissionNames = permissionLookup.GetValueOrDefault(child.Id) ?? [];
            var granted = IsGranted(child.AuthorizationMode, permissionNames, grantedPermissionLookup);

            if (child.Type == MenuType.Button)
            {
                if (granted && !string.IsNullOrWhiteSpace(child.ButtonCode))
                {
                    authBtnList.Add(child.ButtonCode);
                }

                continue;
            }

            var childRoutes = BuildRoutes(child.Id, menus, permissionLookup, grantedPermissionLookup, authBtnList);
            if (!granted && childRoutes.Count == 0)
            {
                continue;
            }

            routes.Add(new MenuRouteDto
            {
                Path = child.Path ?? string.Empty,
                Name = child.RouteName ?? child.Name,
                Redirect = child.Redirect,
                Component = child.Component,
                Meta = new MenuRouteMetaDto
                {
                    Title = child.Title,
                    IsLink = (child.IsExternal || child.IsIframe) ? child.ExternalUrl ?? string.Empty : string.Empty,
                    IsHide = !child.Visible,
                    IsKeepAlive = child.KeepAlive,
                    IsAffix = child.Affix,
                    IsIframe = child.IsIframe,
                    Roles = [],
                    Icon = child.Icon ?? string.Empty
                },
                Children = childRoutes
            });
        }

        return routes;
    }

    private static bool IsGranted(
        MenuAuthorizationMode authorizationMode,
        List<string> permissionNames,
        Dictionary<string, bool> grantedPermissionLookup)
    {
        if (authorizationMode == MenuAuthorizationMode.Anonymous)
        {
            return true;
        }

        if (permissionNames.Count == 0)
        {
            return false;
        }

        return authorizationMode == MenuAuthorizationMode.PermissionAll
            ? permissionNames.All(x => grantedPermissionLookup.GetValueOrDefault(x))
            : permissionNames.Any(x => grantedPermissionLookup.GetValueOrDefault(x));
    }
}