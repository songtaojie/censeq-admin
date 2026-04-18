using Volo.Abp.Application.Services;

namespace Censeq.PermissionManagement;

/// <summary>权限定义管理应用服务接口</summary>
public interface IPermissionDefinitionAppService : IApplicationService
{
    /// <summary>获取所有权限组列表</summary>
    Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync();

    /// <summary>更新权限组显示名称</summary>
    Task<PermissionGroupDefinitionDto> UpdateGroupAsync(string groupName, UpdatePermissionGroupDefinitionDto input);

    /// <summary>获取指定权限组下的权限项列表</summary>
    Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName);

    /// <summary>更新权限项显示名称</summary>
    Task<PermissionDefinitionDto> UpdatePermissionAsync(string name, UpdatePermissionDefinitionDto input);
}
