using System.Linq;
using Censeq.Admin.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SimpleStateChecking;

namespace Censeq.Admin.Menus;

[Authorize(AdminPermissions.Menus.Default)]
public class MenuAppService : AdminAppService, IMenuAppService
{
    private readonly IRepository<Menu, Guid> _menuRepository;
    private readonly IRepository<MenuPermission, Guid> _menuPermissionRepository;
    private readonly MenuManager _menuManager;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly ISimpleStateCheckerManager<PermissionDefinition> _simpleStateCheckerManager;

    public MenuAppService(
        IRepository<Menu, Guid> menuRepository,
        IRepository<MenuPermission, Guid> menuPermissionRepository,
        MenuManager menuManager,
        IPermissionDefinitionManager permissionDefinitionManager,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager)
    {
        _menuRepository = menuRepository;
        _menuPermissionRepository = menuPermissionRepository;
        _menuManager = menuManager;
        _permissionDefinitionManager = permissionDefinitionManager;
        _simpleStateCheckerManager = simpleStateCheckerManager;
    }

    public virtual async Task<ListResultDto<MenuPermissionGroupDto>> GetPermissionGroupsAsync(Guid? menuId = null, Guid? parentId = null)
    {
        // 优先使用菜单自身的 PermissionGroups 字段精确过滤
        HashSet<string>? allowedGroupNames = null;

        if (menuId.HasValue)
        {
            var menu = await _menuRepository.FindAsync(menuId.Value);
            if (menu?.PermissionGroups != null)
            {
                allowedGroupNames = ParsePermissionGroups(menu.PermissionGroups);
            }
        }

        // 新增模式：从父菜单获取 PermissionGroups
        if (allowedGroupNames == null && parentId.HasValue)
        {
            var parentMenu = await _menuRepository.FindAsync(parentId.Value);
            if (parentMenu?.PermissionGroups != null)
            {
                allowedGroupNames = ParsePermissionGroups(parentMenu.PermissionGroups);
            }
        }

        var result = new List<MenuPermissionGroupDto>();
        var multiTenancySide = CurrentTenant.GetMultiTenancySide();

        foreach (var group in await _permissionDefinitionManager.GetGroupsAsync())
        {
            if (allowedGroupNames != null && !allowedGroupNames.Contains(group.Name))
            {
                continue;
            }

            var permissions = new List<MenuPermissionDefinitionDto>();
            var availableDefinitions = group.GetPermissionsWithChildren()
                .Where(x => x.IsEnabled)
                .Where(x => x.MultiTenancySide.HasFlag(multiTenancySide))
                .ToList();

            foreach (var permission in availableDefinitions)
            {
                if (!await _simpleStateCheckerManager.IsEnabledAsync(permission))
                {
                    continue;
                }

                permissions.Add(new MenuPermissionDefinitionDto
                {
                    Name = permission.Name,
                    DisplayName = permission.DisplayName?.Localize(StringLocalizerFactory) ?? permission.Name,
                    ParentName = permission.Parent?.Name
                });
            }

            if (permissions.Count == 0)
            {
                continue;
            }

            result.Add(new MenuPermissionGroupDto
            {
                Name = group.Name,
                DisplayName = group.DisplayName.Localize(StringLocalizerFactory),
                Permissions = permissions
            });
        }

        return new ListResultDto<MenuPermissionGroupDto>(result);
    }

    private static HashSet<string> ParsePermissionGroups(string permissionGroups)
    {
        return new HashSet<string>(
            permissionGroups.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            StringComparer.Ordinal);
    }

    public virtual async Task<ListResultDto<string>> GetReferencedPermissionNamesAsync()
    {
        var queryable = await _menuPermissionRepository.GetQueryableAsync();
        var permissionNames = await AsyncExecuter.ToListAsync(
            queryable.Select(x => x.PermissionName).Distinct());
        return new ListResultDto<string>(permissionNames);
    }

    /// <summary>
    /// 获取租户作用域菜单所引用的权限名称集合（用于平台配置租户权限范围时的可选项过滤）
    /// </summary>
    public virtual async Task<ListResultDto<string>> GetTenantScopePermissionNamesAsync()
    {
        var menuQueryable = await _menuRepository.GetQueryableAsync();
        var permQueryable = await _menuPermissionRepository.GetQueryableAsync();

        var permissionNames = await AsyncExecuter.ToListAsync(
            from mp in permQueryable
            join m in menuQueryable on mp.MenuId equals m.Id
            where m.TenantId == null && m.Scope == MenuScope.Tenant
            select mp.PermissionName into name
            select name);

        return new ListResultDto<string>(permissionNames.Distinct().ToList());
    }

    public virtual async Task<ListResultDto<MenuTreeItemDto>> GetTreeAsync()
    {
        var menus = await GetManagementMenusAsync();
        var permissionLookup = await GetPermissionLookupAsync(menus.Select(x => x.Id).ToList());
        var tree = BuildTree(null, menus, permissionLookup);
        return new ListResultDto<MenuTreeItemDto>(tree);
    }

    public virtual async Task<MenuDetailDto> GetAsync(Guid id)
    {
        var menu = await _menuRepository.GetAsync(id);
        EnsureTenantScope(menu);

        var permissions = await GetPermissionsAsync(id);
        return MapToDetailDto(menu, permissions);
    }

    [Authorize(AdminPermissions.Menus.Create)]
    public virtual async Task<MenuDetailDto> CreateAsync(CreateMenuDto input)
    {
        await ValidatePermissionConfigurationAsync(input.AuthorizationMode, input.PermissionNames);

        var menu = new Menu(GuidGenerator.Create(), CurrentTenant.Id, input.Name, input.Title, input.Type);
        ApplyInput(menu, input);

        await _menuManager.ValidateAsync(menu);
        await _menuRepository.InsertAsync(menu, autoSave: true);
        await ReplacePermissionsAsync(menu, input.PermissionNames);

        return await GetAsync(menu.Id);
    }

    [Authorize(AdminPermissions.Menus.Update)]
    public virtual async Task<MenuDetailDto> UpdateAsync(Guid id, UpdateMenuDto input)
    {
        await ValidatePermissionConfigurationAsync(input.AuthorizationMode, input.PermissionNames);

        var menu = await _menuRepository.GetAsync(id);
        EnsureTenantScope(menu);
        if (!string.IsNullOrWhiteSpace(input.ConcurrencyStamp))
        {
            menu.ConcurrencyStamp = input.ConcurrencyStamp;
        }
        ApplyInput(menu, input);

        await _menuManager.ValidateAsync(menu, id);
        await _menuRepository.UpdateAsync(menu, autoSave: true);
        await ReplacePermissionsAsync(menu, input.PermissionNames);

        return await GetAsync(id);
    }

    [Authorize(AdminPermissions.Menus.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var menu = await _menuRepository.GetAsync(id);
        EnsureTenantScope(menu);

        var menus = await GetManagementMenusAsync();
        var descendantIds = CollectMenuIds(id, menus).Reverse<Guid>().ToList();
        foreach (var menuId in descendantIds)
        {
            var permissions = await GetPermissionsAsync(menuId);
            foreach (var permission in permissions)
            {
                await _menuPermissionRepository.DeleteAsync(permission, autoSave: true);
            }

            var current = await _menuRepository.GetAsync(menuId);
            await _menuRepository.DeleteAsync(current, autoSave: true);
        }
    }

    [Authorize(AdminPermissions.Menus.ManageStatus)]
    public virtual async Task<MenuDetailDto> SetStatusAsync(Guid id, SetMenuStatusDto input)
    {
        var menu = await _menuRepository.GetAsync(id);
        EnsureTenantScope(menu);

        menu.SetStatus(input.Status);
        await _menuRepository.UpdateAsync(menu, autoSave: true);
        return await GetAsync(id);
    }

    [Authorize(AdminPermissions.Menus.ManageOrder)]
    public virtual async Task<MenuDetailDto> MoveAsync(Guid id, MenuSortDto input)
    {
        var menu = await _menuRepository.GetAsync(id);
        EnsureTenantScope(menu);

        menu.SetParent(input.ParentId);
        menu.SetSort(input.Sort);
        await _menuManager.ValidateAsync(menu, id);
        await _menuRepository.UpdateAsync(menu, autoSave: true);

        return await GetAsync(id);
    }

    [Authorize(AdminPermissions.Menus.CopyFromHost)]
    public virtual async Task CopyFromHostAsync(CopyTenantMenusDto input)
    {
        if (!CurrentTenant.Id.HasValue)
        {
            throw new AbpException("Only tenant side can copy host menus.");
        }

        var tenantMenus = await GetManagementMenusAsync();
        if (tenantMenus.Count > 0)
        {
            if (!input.ClearExisting)
            {
                throw new AbpException("Current tenant already has its own menus. Clear them first or enable overwrite.");
            }

            var existingIds = tenantMenus.Select(x => x.Id).Reverse().ToList();
            foreach (var menuId in existingIds)
            {
                var permissions = await GetPermissionsAsync(menuId);
                foreach (var permission in permissions)
                {
                    await _menuPermissionRepository.DeleteAsync(permission, autoSave: true);
                }

                var current = await _menuRepository.GetAsync(menuId);
                await _menuRepository.DeleteAsync(current, autoSave: true);
            }
        }

        var hostMenus = await GetMenusByTenantAsync(null);
        var permissionLookup = await GetPermissionLookupAsync(hostMenus.Select(x => x.Id).ToList());
        var childrenLookup = hostMenus
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreationTime)
            .ToLookup(x => x.ParentId);

        await CopyRecursiveAsync(null, null, childrenLookup, permissionLookup);
    }

    private async Task CopyRecursiveAsync(
        Guid? hostParentId,
        Guid? targetParentId,
        ILookup<Guid?, Menu> childrenLookup,
        Dictionary<Guid, List<string>> permissionLookup)
    {
        var children = childrenLookup[hostParentId]
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreationTime)
            .ToList();

        if (children.Count == 0)
        {
            return;
        }

        foreach (var source in children)
        {
            var target = new Menu(GuidGenerator.Create(), CurrentTenant.Id, source.Name, source.Title, source.Type);
            target.SetParent(targetParentId);
            target.SetRouteName(source.RouteName);
            target.SetPath(source.Path);
            target.SetComponent(source.Component);
            target.SetRedirect(source.Redirect);
            target.SetIcon(source.Icon);
            target.SetSort(source.Sort);
            target.SetDisplayOptions(source.Visible, source.KeepAlive, source.Affix);
            target.SetLinkOptions(source.IsExternal, source.ExternalUrl, source.IsIframe);
            target.SetStatus(source.Status);
            target.SetAuthorizationMode(source.AuthorizationMode);
            target.SetRemark(source.Remark);
            target.SetButtonCode(source.ButtonCode);
            target.SetPermissionGroups(source.PermissionGroups);

            await _menuManager.ValidateAsync(target);
            await _menuRepository.InsertAsync(target, autoSave: true);
            await ReplacePermissionsAsync(target, permissionLookup.GetValueOrDefault(source.Id) ?? []);

            await CopyRecursiveAsync(source.Id, target.Id, childrenLookup, permissionLookup);
        }
    }

    private void ApplyInput(Menu menu, CreateMenuDto input)
    {
        menu.SetParent(input.ParentId);
        menu.SetName(input.Name);
        menu.SetTitle(input.Title);
        menu.SetType(input.Type);
        menu.SetRouteName(input.RouteName);
        menu.SetPath(input.Path);
        menu.SetComponent(input.Component);
        menu.SetRedirect(input.Redirect);
        menu.SetIcon(input.Icon);
        menu.SetSort(input.Sort);
        menu.SetDisplayOptions(input.Visible, input.KeepAlive, input.Affix);
        menu.SetLinkOptions(input.IsExternal, input.ExternalUrl, input.IsIframe);
        menu.SetStatus(input.Status);
        menu.SetAuthorizationMode(input.AuthorizationMode);
        menu.SetScope(input.Scope);
        menu.SetRemark(input.Remark);
        menu.SetButtonCode(input.ButtonCode);
        menu.SetPermissionGroups(input.PermissionGroups);
    }

    private void EnsureTenantScope(Menu menu)
    {
        if (menu.TenantId != CurrentTenant.Id)
        {
            throw new AbpException("The target menu does not belong to the current tenant scope.");
        }
    }

    private async Task ValidatePermissionConfigurationAsync(MenuAuthorizationMode authorizationMode, IEnumerable<string> permissionNames)
    {
        var normalized = NormalizePermissions(permissionNames);
        if (authorizationMode != MenuAuthorizationMode.Anonymous && normalized.Count == 0)
        {
            throw new AbpException("Permission based menu nodes must bind at least one ABP permission.");
        }

        if (normalized.Count > 0)
        {
            var allDefinitions = await _permissionDefinitionManager.GetPermissionsAsync();
            var definedNames = new HashSet<string>(allDefinitions.Select(x => x.Name), StringComparer.Ordinal);
            var invalidNames = normalized.Where(x => !definedNames.Contains(x)).ToList();
            if (invalidNames.Count > 0)
            {
                throw new AbpException($"The following permission names do not exist: {string.Join(", ", invalidNames)}");
            }
        }
    }

    private async Task ReplacePermissionsAsync(Menu menu, IEnumerable<string> permissionNames)
    {
        var existingPermissions = await GetPermissionsAsync(menu.Id);
        foreach (var permission in existingPermissions)
        {
            await _menuPermissionRepository.DeleteAsync(permission, autoSave: true);
        }

        var normalizedPermissions = NormalizePermissions(permissionNames);
        foreach (var permissionName in normalizedPermissions)
        {
            var permission = new MenuPermission(GuidGenerator.Create(), menu.Id, permissionName);
            await _menuPermissionRepository.InsertAsync(permission, autoSave: true);
        }
    }

    private static List<string> NormalizePermissions(IEnumerable<string> permissionNames)
    {
        return permissionNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private async Task<List<Menu>> GetManagementMenusAsync()
    {
        return await GetMenusByTenantAsync(CurrentTenant.Id);
    }

    private async Task<List<Menu>> GetMenusByTenantAsync(Guid? tenantId)
    {
        var queryable = await _menuRepository.GetQueryableAsync();
        return await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Sort)
                .ThenBy(x => x.CreationTime));
    }

    private async Task<List<MenuPermission>> GetPermissionsAsync(Guid menuId)
    {
        var queryable = await _menuPermissionRepository.GetQueryableAsync();
        return await AsyncExecuter.ToListAsync(queryable.Where(x => x.MenuId == menuId).OrderBy(x => x.PermissionName));
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

    private List<MenuTreeItemDto> BuildTree(Guid? parentId, List<Menu> menus, Dictionary<Guid, List<string>> permissionLookup)
    {
        return menus.Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.CreationTime)
            .Select(x =>
            {
                var item = new MenuTreeItemDto
                {
                    Id = x.Id,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    Title = x.Title,
                    RouteName = x.RouteName,
                    Path = x.Path,
                    Component = x.Component,
                    Icon = x.Icon,
                    Type = x.Type,
                    Sort = x.Sort,
                    Visible = x.Visible,
                    Status = x.Status,
                    AuthorizationMode = x.AuthorizationMode,
                    Scope = x.Scope,
                    ButtonCode = x.ButtonCode,
                    PermissionGroups = x.PermissionGroups,
                    PermissionNames = permissionLookup.GetValueOrDefault(x.Id) ?? []
                };
                item.Children = BuildTree(x.Id, menus, permissionLookup);
                return item;
            })
            .ToList();
    }

    private MenuDetailDto MapToDetailDto(Menu menu, List<MenuPermission> permissions)
    {
        return new MenuDetailDto
        {
            Id = menu.Id,
            TenantId = menu.TenantId,
            ParentId = menu.ParentId,
            Name = menu.Name,
            Title = menu.Title,
            RouteName = menu.RouteName,
            Path = menu.Path,
            Component = menu.Component,
            Redirect = menu.Redirect,
            Icon = menu.Icon,
            Type = menu.Type,
            Sort = menu.Sort,
            Visible = menu.Visible,
            KeepAlive = menu.KeepAlive,
            Affix = menu.Affix,
            IsExternal = menu.IsExternal,
            ExternalUrl = menu.ExternalUrl,
            IsIframe = menu.IsIframe,
            Status = menu.Status,
            AuthorizationMode = menu.AuthorizationMode,
            Scope = menu.Scope,
            Remark = menu.Remark,
            ButtonCode = menu.ButtonCode,
            PermissionGroups = menu.PermissionGroups,
            PermissionNames = permissions.Select(x => x.PermissionName).OrderBy(x => x).ToList(),
            ConcurrencyStamp = menu.ConcurrencyStamp
        };
    }

    private static List<Guid> CollectMenuIds(Guid rootId, List<Menu> menus)
    {
        var result = new List<Guid> { rootId };
        var children = menus.Where(x => x.ParentId == rootId).Select(x => x.Id).ToList();
        foreach (var childId in children)
        {
            result.AddRange(CollectMenuIds(childId, menus));
        }

        return result;
    }
}