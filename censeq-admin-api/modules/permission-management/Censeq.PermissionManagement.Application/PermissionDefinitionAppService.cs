using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement;

/// <summary>权限定义管理应用服务</summary>
[Authorize(PermissionManagementPermissions.DefinitionManagement)]
public class PermissionDefinitionAppService : ApplicationService, IPermissionDefinitionAppService
{
    private readonly IPermissionGroupRepository _groupRepo;
    private readonly IPermissionDefinitionRecordRepository _permissionRepo;

    /// <summary>
    /// 初始化 PermissionDefinitionAppService 实例。
    /// </summary>
    /// <param name="groupRepo">groupRepo。</param>
    /// <param name="permissionRepo">permissionRepo。</param>
    public PermissionDefinitionAppService(
        IPermissionGroupRepository groupRepo,
        IPermissionDefinitionRecordRepository permissionRepo)
    {
        _groupRepo = groupRepo;
        _permissionRepo = permissionRepo;
    }

    /// <inheritdoc/>
    /// <summary>
    /// 获取权限组定义列表。
    /// </summary>
    /// <returns>权限组定义列表。</returns>
    public async Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync()
    {
        var groups = await _groupRepo.GetListAsync();
        return groups
            .Select(g => new PermissionGroupDefinitionDto
            {
                Id = g.Id,
                Name = g.Name,
                DisplayName = g.DisplayName
            })
            .OrderBy(g => g.Name)
            .ToList();
    }

    /// <inheritdoc/>
    /// <summary>
    /// 更新权限组显示名称。
    /// </summary>
    /// <param name="groupName">权限组名称。</param>
    /// <param name="input">更新请求数据。</param>
    /// <returns>更新后的权限组定义。</returns>
    public async Task<PermissionGroupDefinitionDto> UpdateGroupAsync(
        string groupName, UpdatePermissionGroupDefinitionDto input)
    {
        var allGroups = await _groupRepo.GetListAsync();
        var group = allGroups.FirstOrDefault(g => g.Name == groupName);
        if (group == null)
            throw new EntityNotFoundException(typeof(PermissionGroup), groupName);

        group.DisplayName = input.DisplayName.Trim();
        await _groupRepo.UpdateAsync(group);

        return new PermissionGroupDefinitionDto
        {
            Id = group.Id,
            Name = group.Name,
            DisplayName = group.DisplayName
        };
    }

    /// <inheritdoc/>
    /// <summary>
    /// 获取指定权限组下的权限定义列表。
    /// </summary>
    /// <param name="groupName">权限组名称。</param>
    /// <returns>权限定义列表。</returns>
    public async Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName)
    {
        var allGroups = await _groupRepo.GetListAsync();
        var groupExists = allGroups.FirstOrDefault(g => g.Name == groupName);
        if (groupExists == null)
            throw new EntityNotFoundException(typeof(PermissionGroup), groupName);

        var allPermissions = await _permissionRepo.GetListAsync();
        var permissions = allPermissions.Where(p => p.GroupName == groupName).ToList();
        return permissions
            .Select(p => new PermissionDefinitionDto
            {
                Id = p.Id,
                GroupName = p.GroupName,
                Name = p.Name,
                ParentName = p.ParentName,
                DisplayName = p.DisplayName,
                IsEnabled = p.IsEnabled
            })
            .OrderBy(p => p.Name)
            .ToList();
    }

    /// <inheritdoc/>
    /// <summary>
    /// 更新权限定义显示名称。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="input">更新请求数据。</param>
    /// <returns>更新后的权限定义。</returns>
    public async Task<PermissionDefinitionDto> UpdatePermissionAsync(
        string name, UpdatePermissionDefinitionDto input)
    {
        var permission = await _permissionRepo.FindByNameAsync(name);
        if (permission == null)
            throw new EntityNotFoundException(typeof(PermissionDefinitionRecord), name);

        permission.DisplayName = input.DisplayName.Trim();
        await _permissionRepo.UpdateAsync(permission);

        return new PermissionDefinitionDto
        {
            Id = permission.Id,
            GroupName = permission.GroupName,
            Name = permission.Name,
            ParentName = permission.ParentName,
            DisplayName = permission.DisplayName,
            IsEnabled = permission.IsEnabled
        };
    }
}
