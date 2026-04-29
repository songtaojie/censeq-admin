using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Censeq.Admin.Menus;

public class MenuDataSeedContributor : DomainService, IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Menu, Guid> _menuRepository;
    private readonly IRepository<MenuPermission, Guid> _menuPermissionRepository;
    private readonly IDataFilter _dataFilter;

    public MenuDataSeedContributor(
        IRepository<Menu, Guid> menuRepository,
        IRepository<MenuPermission, Guid> menuPermissionRepository,
        IDataFilter dataFilter)
    {
        _menuRepository = menuRepository;
        _menuPermissionRepository = menuPermissionRepository;
        _dataFilter = dataFilter;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId.HasValue)
        {
            // 租户上下文：自动将 host 侧 Tenant scope 菜单复制到当前租户（幂等，已有菜单时跳过）
            await SeedTenantMenusAsync(context.TenantId.Value);
            return;
        }

        // 预加载所有已存在的 host 菜单，并在种子执行时同步更新已有记录。
        var queryable = await _menuRepository.GetQueryableAsync();
        var existingMenus = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.TenantId == null));

        var menuByKey = new Dictionary<string, Menu>(StringComparer.Ordinal);
        foreach (var existing in existingMenus)
        {
            if (!menuByKey.ContainsKey(existing.Name))
            {
                menuByKey[existing.Name] = existing;
            }
        }

        foreach (var definition in GetHostMenuDefinitions())
        {
            var parentId = definition.ParentKey is null
                ? (Guid?)null
                : menuByKey.TryGetValue(definition.ParentKey, out var parentMenu)
                    ? parentMenu.Id
                    : (Guid?)null;

            var menu = FindExistingMenu(existingMenus, menuByKey, definition);
            if (menu == null)
            {
                menu = new Menu(Guid.NewGuid(), null, definition.Name, definition.Title, definition.Type);
                ApplyDefinition(menu, definition, parentId);
                await _menuRepository.InsertAsync(menu, autoSave: true);
                existingMenus.Add(menu);
            }
            else
            {
                ApplyDefinition(menu, definition, parentId);
                await _menuRepository.UpdateAsync(menu, autoSave: true);
            }

            await ReplacePermissionsAsync(menu, definition.PermissionNames);

            menuByKey[definition.Key] = menu;
        }
    }

    private static Menu? FindExistingMenu(
        List<Menu> existingMenus,
        Dictionary<string, Menu> menuByKey,
        SeedMenuDefinition definition)
    {
        if (menuByKey.TryGetValue(definition.Key, out var menu))
        {
            return menu;
        }

        return existingMenus.FirstOrDefault(x => x.Path == definition.Path && x.Component == definition.Component);
    }

    private static void ApplyDefinition(Menu menu, SeedMenuDefinition definition, Guid? parentId)
    {
        menu.SetParent(parentId);
        menu.SetName(definition.Name);
        menu.SetTitle(definition.Title);
        menu.SetType(definition.Type);
        menu.SetRouteName(definition.RouteName);
        menu.SetPath(definition.Path);
        menu.SetComponent(definition.Component);
        menu.SetRedirect(definition.Redirect);
        menu.SetIcon(definition.Icon);
        menu.SetSort(definition.Sort);
        menu.SetDisplayOptions(definition.Visible, definition.KeepAlive, definition.Affix);
        menu.SetLinkOptions(false, null, false);
        menu.SetStatus(true);
        menu.SetAuthorizationMode(definition.AuthorizationMode);
        menu.SetScope(definition.Scope);
        menu.SetPermissionGroups(definition.PermissionGroups);
    }

    /// <summary>
    /// 将 host 侧 Tenant scope 菜单复制到指定租户。幂等：租户已有菜单时直接跳过。
    /// </summary>
    public virtual async Task SeedTenantMenusAsync(Guid tenantId)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            // 幂等检查：包含软删除记录——只要曾经存在过菜单（含已删除）就跳过，
            // 避免与唯一索引上的软删除行发生冲突
            bool hasAny;
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var anyList = await AsyncExecuter.ToListAsync(
                    (await _menuRepository.GetQueryableAsync())
                        .Where(x => x.TenantId == tenantId)
                        .Take(1));
                hasAny = anyList.Count > 0;
            }
            if (hasAny)
                return;

            // 读取 host 侧 Tenant scope 菜单
            var hostMenus = await AsyncExecuter.ToListAsync(
                (await _menuRepository.GetQueryableAsync())
                    .Where(x => x.TenantId == null && x.Scope == MenuScope.Tenant)
                    .OrderBy(x => x.Sort)
                    .ThenBy(x => x.CreationTime));

            if (hostMenus.Count == 0)
                return;

            var hostMenuIds = hostMenus.Select(x => x.Id).ToList();
            var hostPermissions = await AsyncExecuter.ToListAsync(
                (await _menuPermissionRepository.GetQueryableAsync())
                    .Where(x => hostMenuIds.Contains(x.MenuId)));

            var idMapping = new Dictionary<Guid, Guid>();
            foreach (var source in hostMenus)
            {
                var newId = Guid.NewGuid();
                idMapping[source.Id] = newId;

                var newParentId = source.ParentId.HasValue && idMapping.TryGetValue(source.ParentId.Value, out var mappedId)
                    ? mappedId
                    : (Guid?)null;

                var target = new Menu(newId, tenantId, source.Name, source.Title, source.Type);
                target.SetParent(newParentId);
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
                target.SetScope(source.Scope);
                target.SetRemark(source.Remark);
                target.SetButtonCode(source.ButtonCode);
                target.SetPermissionGroups(source.PermissionGroups);

                await _menuRepository.InsertAsync(target, autoSave: true);

                foreach (var perm in hostPermissions.Where(p => p.MenuId == source.Id))
                {
                    await _menuPermissionRepository.InsertAsync(
                        new MenuPermission(Guid.NewGuid(), newId, perm.PermissionName),
                        autoSave: true);
                }
            }
        }
    }

    private async Task ReplacePermissionsAsync(Menu menu, IReadOnlyCollection<string> permissionNames)
    {
        var queryable = await _menuPermissionRepository.GetQueryableAsync();
        var existingPermissions = await AsyncExecuter.ToListAsync(queryable.Where(x => x.MenuId == menu.Id));
        foreach (var permission in existingPermissions)
        {
            await _menuPermissionRepository.DeleteAsync(permission, autoSave: true);
        }

        foreach (var permissionName in permissionNames.Distinct(StringComparer.Ordinal))
        {
            var menuPermission = new MenuPermission(Guid.NewGuid(), menu.Id, permissionName);
            await _menuPermissionRepository.InsertAsync(menuPermission, autoSave: true);
        }
    }

    private static IReadOnlyList<SeedMenuDefinition> GetHostMenuDefinitions()
    {
        return new List<SeedMenuDefinition>
        {
            // ── 平台管理 ─────────────────────────────────────────────
            new(
                key: "platform",
                name: "platform",
                title: "平台管理",
                routeName: "platform",
                path: "/platform",
                component: "layout/routerView/parent",
                redirect: "/platform/dashboard",
                icon: "ele-Monitor",
                type: MenuType.Directory,
                sort: 10,
                authorizationMode: MenuAuthorizationMode.Anonymous,
                permissionNames: Array.Empty<string>(),
                scope: MenuScope.Platform),
            new(
                key: "platformDashboard",
                parentKey: "platform",
                name: "platformDashboard",
                title: "平台概览",
                routeName: "platformDashboard",
                path: "/platform/dashboard",
                component: "platform/dashboard/index",
                redirect: null,
                icon: "ele-DataBoard",
                type: MenuType.Menu,
                sort: 0,
                affix: true,
                authorizationMode: MenuAuthorizationMode.Anonymous,
                permissionNames: Array.Empty<string>(),
                scope: MenuScope.Platform),
            new(
                key: "platformUser",
                parentKey: "platform",
                name: "platformUser",
                title: "平台用户管理",
                routeName: "platformUser",
                path: "/platform/users",
                component: "system/user/index",
                redirect: null,
                icon: "iconfont icon-icon-",
                type: MenuType.Menu,
                sort: 5,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqIdentity.Users" },
                scope: MenuScope.Platform,
                permissionGroups: "CenseqIdentity"),
            new(
                key: "platformRole",
                parentKey: "platform",
                name: "platformRole",
                title: "平台角色管理",
                routeName: "platformRole",
                path: "/platform/roles",
                component: "system/role/index",
                redirect: null,
                icon: "ele-ColdDrink",
                type: MenuType.Menu,
                sort: 6,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqIdentity.Roles" },
                scope: MenuScope.Platform,
                permissionGroups: "CenseqIdentity"),
            new(
                key: "systemTenant",
                parentKey: "platform",
                name: "systemTenant",
                title: "租户管理",
                routeName: "systemTenant",
                path: "/platform/tenant",
                component: "system/tenant/index",
                redirect: null,
                icon: "ele-HomeFilled",
                type: MenuType.Menu,
                sort: 10,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "TenantManagement.Tenants" },
                scope: MenuScope.Platform,
                permissionGroups: "TenantManagement"),
            new(
                key: "systemFeature",
                parentKey: "platform",
                name: "systemFeature",
                title: "特性管理",
                routeName: "systemFeature",
                path: "/platform/feature",
                component: "system/feature/index",
                redirect: null,
                icon: "iconfont icon-crew_feature",
                type: MenuType.Menu,
                sort: 20,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqFeatureManagement.ManageHostFeatures" },
                scope: MenuScope.Platform,
                permissionGroups: "CenseqFeatureManagement"),
            new(
                key: "systemMenu",
                parentKey: "platform",
                name: "systemMenu",
                title: "菜单管理",
                routeName: "systemMenu",
                path: "/platform/menu",
                component: "system/menu/index",
                redirect: null,
                icon: "iconfont icon-caidan",
                type: MenuType.Menu,
                sort: 30,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqAdmin.Menus" },
                scope: MenuScope.Platform,
                permissionGroups: "Admin"),
            new(
                key: "systemClaimType",
                parentKey: "platform",
                name: "systemClaimType",
                title: "声明类型管理",
                routeName: "systemClaimType",
                path: "/platform/claim-type",
                component: "system/claim-type/index",
                redirect: null,
                icon: "ele-Collection",
                type: MenuType.Menu,
                sort: 40,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqIdentity.ClaimTypes" },
                scope: MenuScope.Platform,
                permissionGroups: "CenseqIdentity"),
            new(
                key: "systemPermissionDefinition",
                parentKey: "platform",
                name: "systemPermissionDefinition",
                title: "权限定义管理",
                routeName: "systemPermissionDefinition",
                path: "/platform/permission-definition",
                component: "system/permission-definition/index",
                redirect: null,
                icon: "ele-Key",
                type: MenuType.Menu,
                sort: 50,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "PermissionManagement.DefinitionManagement" },
                scope: MenuScope.Platform,
                permissionGroups: "PermissionManagement"),
            new(
                key: "systemAuditLog",
                parentKey: "platform",
                name: "systemAuditLog",
                title: "审计日志",
                routeName: "systemAuditLog",
                path: "/platform/audit-log",
                component: "system/audit-log/index",
                redirect: null,
                icon: "ele-Document",
                type: MenuType.Menu,
                sort: 60,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "AuditLogging.AuditLogs" },
                scope: MenuScope.Platform,
                permissionGroups: "AuditLogging"),

            // ── 企业运营（租户可见，数据自动隔离；平台管理员通过 TenantPermissionGrant 控制可见范围） ─────────────
            new(
                key: "system",
                name: "system",
                title: "企业运营",
                routeName: "system",
                path: "/system",
                component: "layout/routerView/parent",
                redirect: "/system/dashboard",
                icon: "iconfont icon-xitongshezhi",
                type: MenuType.Directory,
                sort: 20,
                authorizationMode: MenuAuthorizationMode.Anonymous,
                permissionNames: Array.Empty<string>(),
                scope: MenuScope.Tenant),
            new(
                key: "systemDashboard",
                parentKey: "system",
                name: "systemDashboard",
                title: "企业概览",
                routeName: "systemDashboard",
                path: "/system/dashboard",
                component: "system/dashboard/index",
                redirect: null,
                icon: "ele-DataAnalysis",
                type: MenuType.Menu,
                sort: 0,
                affix: true,
                authorizationMode: MenuAuthorizationMode.Anonymous,
                permissionNames: Array.Empty<string>(),
                scope: MenuScope.Tenant),
            new(
                key: "systemRole",
                parentKey: "system",
                name: "systemRole",
                title: "角色管理",
                routeName: "systemRole",
                path: "/system/role",
                component: "system/role/index",
                redirect: null,
                icon: "ele-ColdDrink",
                type: MenuType.Menu,
                sort: 10,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqIdentity.Roles" },
                scope: MenuScope.Tenant,
                permissionGroups: "CenseqIdentity"),
            new(
                key: "systemUser",
                parentKey: "system",
                name: "systemUser",
                title: "用户管理",
                routeName: "systemUser",
                path: "/system/user",
                component: "system/user/index",
                redirect: null,
                icon: "iconfont icon-icon-",
                type: MenuType.Menu,
                sort: 20,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqIdentity.Users" },
                scope: MenuScope.Tenant,
                permissionGroups: "CenseqIdentity"),
            new(
                key: "systemDept",
                parentKey: "system",
                name: "systemDept",
                title: "部门管理",
                routeName: "systemDept",
                path: "/system/dept",
                component: "system/dept/index",
                redirect: null,
                icon: "ele-OfficeBuilding",
                type: MenuType.Menu,
                sort: 30,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqIdentity.OrganizationUnits" },
                scope: MenuScope.Tenant,
                permissionGroups: "CenseqIdentity"),
            new(
                key: "systemDic",
                parentKey: "system",
                name: "systemDic",
                title: "数据字典",
                routeName: "systemDic",
                path: "/system/dic",
                component: "system/dic/index",
                redirect: null,
                icon: "ele-SetUp",
                type: MenuType.Menu,
                sort: 40,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: Array.Empty<string>(),
                scope: MenuScope.Tenant,
                permissionGroups: null),
            new(
                key: "systemSettings",
                parentKey: "system",
                name: "systemSettings",
                title: "应用设置",
                routeName: "systemSettings",
                path: "/system/settings",
                component: "system/settings/index",
                redirect: null,
                icon: "ele-Setting",
                type: MenuType.Menu,
                sort: 50,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "SettingManagement.Emailing", "SettingManagement.TimeZone" },
                scope: MenuScope.Tenant,
                permissionGroups: "SettingManagement"),
            new(
                key: "systemTenantMenu",
                parentKey: "system",
                name: "systemTenantMenu",
                title: "菜单配置",
                routeName: "systemTenantMenu",
                path: "/system/tenant-menu",
                component: "system/tenant-menu/index",
                redirect: null,
                icon: "iconfont icon-caidan",
                type: MenuType.Menu,
                sort: 60,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "CenseqAdmin.Menus.CopyFromHost" },
                scope: MenuScope.Tenant,
                permissionGroups: "Admin"),

            // ── 认证中心 ──────────────────────────────────────────────
            new(
                key: "openiddict",
                name: "openiddict",
                title: "认证中心",
                routeName: "openiddict",
                path: "/openiddict",
                component: "layout/routerView/parent",
                redirect: "/openiddict/application",
                icon: "ele-Lock",
                type: MenuType.Directory,
                sort: 30,
                authorizationMode: MenuAuthorizationMode.Anonymous,
                permissionNames: Array.Empty<string>(),
                scope: MenuScope.Platform),
            new(
                key: "openiddictApplication",
                parentKey: "openiddict",
                name: "openiddictApplication",
                title: "应用管理",
                routeName: "openiddictApplication",
                path: "/openiddict/application",
                component: "openiddict/application/index",
                redirect: null,
                icon: "ele-Connection",
                type: MenuType.Menu,
                sort: 10,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "OpenIddict.Applications" },
                scope: MenuScope.Platform,
                permissionGroups: "OpenIddict"),
            new(
                key: "openiddictScope",
                parentKey: "openiddict",
                name: "openiddictScope",
                title: "作用域管理",
                routeName: "openiddictScope",
                path: "/openiddict/scope",
                component: "openiddict/scope/index",
                redirect: null,
                icon: "ele-Collection",
                type: MenuType.Menu,
                sort: 20,
                authorizationMode: MenuAuthorizationMode.Permission,
                permissionNames: new[] { "OpenIddict.Scopes" },
                scope: MenuScope.Platform,
                permissionGroups: "OpenIddict")
        };
    }

    private sealed class SeedMenuDefinition
    {
        public SeedMenuDefinition(
            string key,
            string name,
            string title,
            string routeName,
            string path,
            string component,
            string? redirect,
            string icon,
            MenuType type,
            int sort,
            MenuAuthorizationMode authorizationMode,
            IReadOnlyList<string> permissionNames,
            MenuScope scope,
            string? parentKey = null,
            bool visible = true,
            bool keepAlive = true,
            bool affix = false,
            string? permissionGroups = null)
        {
            Key = key;
            ParentKey = parentKey;
            Name = name;
            Title = title;
            RouteName = routeName;
            Path = path;
            Component = component;
            Redirect = redirect;
            Icon = icon;
            Type = type;
            Sort = sort;
            Visible = visible;
            KeepAlive = keepAlive;
            Affix = affix;
            AuthorizationMode = authorizationMode;
            PermissionNames = permissionNames;
            Scope = scope;
            PermissionGroups = permissionGroups;
        }

        public string Key { get; }
        public string? ParentKey { get; }
        public string Name { get; }
        public string Title { get; }
        public string RouteName { get; }
        public string Path { get; }
        public string Component { get; }
        public string? Redirect { get; }
        public string Icon { get; }
        public MenuType Type { get; }
        public int Sort { get; }
        public bool Visible { get; }
        public bool KeepAlive { get; }
        public bool Affix { get; }
        public MenuAuthorizationMode AuthorizationMode { get; }
        public IReadOnlyList<string> PermissionNames { get; }
        public MenuScope Scope { get; }
        public string? PermissionGroups { get; }
    }
}