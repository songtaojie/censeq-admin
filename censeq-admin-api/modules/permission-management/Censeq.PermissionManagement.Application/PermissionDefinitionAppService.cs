using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement;

/// <summary>权限定义管理应用服务</summary>
[Authorize(PermissionManagementPermissions.DefinitionManagement)]
public class PermissionDefinitionAppService : ApplicationService, IPermissionDefinitionAppService
{
    private readonly IPermissionGroupDefinitionRecordRepository _groupRepo;
    private readonly IPermissionDefinitionRecordRepository _permissionRepo;

    public PermissionDefinitionAppService(
        IPermissionGroupDefinitionRecordRepository groupRepo,
        IPermissionDefinitionRecordRepository permissionRepo)
    {
        _groupRepo = groupRepo;
        _permissionRepo = permissionRepo;
    }

    /// <inheritdoc/>
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
    public async Task<PermissionGroupDefinitionDto> UpdateGroupAsync(
        string groupName, UpdatePermissionGroupDefinitionDto input)
    {
        var allGroups = await _groupRepo.GetListAsync();
        var group = allGroups.FirstOrDefault(g => g.Name == groupName);
        if (group == null)
            throw new EntityNotFoundException(typeof(PermissionGroupDefinitionRecord), groupName);

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
    public async Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName)
    {
        var allGroups = await _groupRepo.GetListAsync();
        var groupExists = allGroups.FirstOrDefault(g => g.Name == groupName);
        if (groupExists == null)
            throw new EntityNotFoundException(typeof(PermissionGroupDefinitionRecord), groupName);

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
