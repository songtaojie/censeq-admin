using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Services;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SimpleStateChecking;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限应用服务。
/// 负责面向前端组装指定提供者的权限树，并把授权变更委托给领域层处理。
/// </summary>
[Authorize]
public class PermissionAppService : ApplicationService, IPermissionAppService
{
    /// <summary>
    /// 权限管理配置
    /// </summary>
    protected PermissionManagementOptions Options { get; }
    /// <summary>
    /// 权限管理
    /// </summary>
    protected IPermissionManager PermissionManager { get; }
    /// <summary>
    /// 权限定义管理
    /// </summary>
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
    /// <summary>
    /// 状态检查管理
    /// </summary>
    protected ISimpleStateCheckerManager<PermissionDefinition> SimpleStateCheckerManager { get; }

    /// <summary>
    /// 权限应用服务
    /// </summary>
    /// <param name="permissionManager">权限管理</param>
    /// <param name="permissionDefinitionManager">权限定义管理</param>
    /// <param name="options">权限配置</param>
    /// <param name="simpleStateCheckerManager">状态检查管理</param>
    public PermissionAppService(
        IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager,
        IOptions<PermissionManagementOptions> options,
        ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager)
    {
        Options = options.Value;
        PermissionManager = permissionManager;
        PermissionDefinitionManager = permissionDefinitionManager;
        SimpleStateCheckerManager = simpleStateCheckerManager;
    }

    /// <summary>
    /// 获取指定权限提供者和提供者标识对应的权限列表。
    /// 会过滤禁用权限、不匹配当前租户侧的权限，以及状态检查未通过的权限。
    /// </summary>
    /// <param name="providerName">权限提供者名称，例如角色、用户或客户端。</param>
    /// <param name="providerKey">权限提供者标识，例如角色 Id 或用户 Id。</param>
    /// <returns>按权限组组织后的权限授予状态。</returns>
    public virtual async Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
    {
        await CheckProviderPolicy(providerName);

        var result = new GetPermissionListResultDto
        {
            EntityDisplayName = providerKey,
            Groups = []
        };

        var multiTenancySide = CurrentTenant.GetMultiTenancySide();

        foreach (var group in await PermissionDefinitionManager.GetGroupsAsync())
        {
            var groupDto = CreatePermissionGroupDto(group);

            var neededCheckPermissions = new List<PermissionDefinition>();

            var permissions = group.GetPermissionsWithChildren()
                .Where(x => x.IsEnabled)
                .Where(x => x.Providers.Count == 0 || x.Providers.Contains(providerName))
                .Where(x => x.MultiTenancySide.HasFlag(multiTenancySide));

            foreach (var permission in permissions)
            {
                if (permission.Parent != null && !neededCheckPermissions.Contains(permission.Parent))
                {
                    continue;
                }

                if (await SimpleStateCheckerManager.IsEnabledAsync(permission))
                {
                    neededCheckPermissions.Add(permission);
                }
            }

            if (neededCheckPermissions.Count == 0)
            {
                continue;
            }

            var grantInfoDtos = neededCheckPermissions
                .Select(CreatePermissionGrantInfoDto)
                .ToList();

            var multipleGrantInfo = await PermissionManager.GetAsync(neededCheckPermissions.Select(x => x.Name).ToArray(), providerName, providerKey);

            foreach (var grantInfo in multipleGrantInfo.Result)
            {
                var grantInfoDto = grantInfoDtos.First(x => x.Name == grantInfo.Name);

                grantInfoDto.IsGranted = grantInfo.IsGranted;

                foreach (var provider in grantInfo.Providers)
                {
                    grantInfoDto.GrantedProviders!.Add(new ProviderInfoDto
                    {
                        ProviderName = provider.Name,
                        ProviderKey = provider.Key,
                    });
                }

                groupDto.Permissions!.Add(grantInfoDto);
            }

            if (groupDto.Permissions!.Any())
            {
                result.Groups.Add(groupDto);
            }
        }

        return result;
    }

    /// <summary>
    /// 创建权限授予信息。
    /// </summary>
    /// <param name="permission">权限定义</param>
    /// <returns>包含显示名称、父级和可用提供者信息的权限 DTO。</returns>
    private PermissionGrantInfoDto CreatePermissionGrantInfoDto(PermissionDefinition permission)
    {
        return new PermissionGrantInfoDto
        {
            Name = permission.Name,
            DisplayName = permission.DisplayName == null? string.Empty: permission.DisplayName.Localize(StringLocalizerFactory),
            ParentName = permission.Parent?.Name,
            AllowedProviders = permission.Providers,
            GrantedProviders = []
        };
    }

    /// <summary>
    /// 创建权限组。
    /// </summary>
    /// <param name="group">权限组定义。</param>
    /// <returns>可返回给前端的权限组 DTO。</returns>
    private PermissionGroupDto CreatePermissionGroupDto(PermissionGroupDefinition group)
    {
        var localizableDisplayName = group.DisplayName as LocalizableString;

        return new PermissionGroupDto
        {
            Name = group.Name,
            DisplayName = group.DisplayName.Localize(StringLocalizerFactory),
            DisplayNameKey = localizableDisplayName?.Name,
            DisplayNameResource = localizableDisplayName?.ResourceType != null
                ? LocalizationResourceNameAttribute.GetName(localizableDisplayName.ResourceType)
                : null,
            Permissions = []
        };
    }

    /// <summary>
    /// 更新指定提供者下的一组权限授予状态。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="input">待更新的权限授予状态。</param>
    /// <returns>异步任务。</returns>
    public virtual async Task UpdateAsync(string providerName, string providerKey, UpdatePermissionsDto input)
    {
        await CheckProviderPolicy(providerName);

        foreach (var permissionDto in input.Permissions)
        {
            await PermissionManager.SetAsync(permissionDto.Name, providerName, providerKey, permissionDto.IsGranted);
        }
    }

    /// <summary>
    /// 检查当前用户是否具备管理指定权限提供者的策略。
    /// </summary>
    /// <param name="providerName">提供者名称</param>
    /// <returns>异步任务。</returns>
    /// <exception cref="BusinessException">未配置提供者策略时抛出。</exception>
    protected virtual async Task CheckProviderPolicy(string providerName)
    {
        var policyName = Options.ProviderPolicies.GetOrDefault(providerName);
        if (policyName.IsNullOrEmpty())
        {
            throw new BusinessException($"没有为提供程序'{providerName}'定义获取/设置权限的策略。 使用{nameof(PermissionManagementOptions)}来配置策略。");
        }

        await AuthorizationService.CheckAsync(policyName);
    }
}
