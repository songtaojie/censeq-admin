using Censeq.PermissionManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.SimpleStateChecking;

namespace Censeq.PermissionManagement;

/// <summary>
/// 动态权限定义存储的进程内缓存实现。
/// 负责把持久化的权限记录还原为 ABP 权限定义对象。
/// </summary>
public class DynamicPermissionDefinitionStoreInMemoryCache : IDynamicPermissionDefinitionStoreInMemoryCache,ISingletonDependency
{
    /// <summary>
    /// 缓存标记
    /// </summary>
    public string CacheStamp { get; set; } = string.Empty;

    /// <summary>
    /// 权限组定义
    /// </summary>
    protected IDictionary<string, PermissionGroupDefinition> PermissionGroupDefinitions { get; }

    /// <summary>
    /// 权限定义
    /// </summary>
    protected IDictionary<string, PermissionDefinition> PermissionDefinitions { get; }

    /// <summary>
    /// 状态检查器序列化器
    /// </summary>
    protected ISimpleStateCheckerSerializer StateCheckerSerializer { get; }

    /// <summary>
    /// 本地化字符串序列化器
    /// </summary>
    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

    /// <summary>
    /// 同步信号量
    /// </summary>
    public SemaphoreSlim SyncSemaphore { get; } = new(1, 1);

    /// <summary>
    /// 最后检查时间
    /// </summary>
    public DateTime? LastCheckTime { get; set; }

    /// <summary>
    /// 初始化动态权限定义进程内缓存。
    /// </summary>
    /// <param name="stateCheckerSerializer">权限状态检查器序列化器。</param>
    /// <param name="localizableStringSerializer">本地化字符串序列化器。</param>
    public DynamicPermissionDefinitionStoreInMemoryCache(ISimpleStateCheckerSerializer stateCheckerSerializer,
        ILocalizableStringSerializer localizableStringSerializer)
    {
        StateCheckerSerializer = stateCheckerSerializer;
        LocalizableStringSerializer = localizableStringSerializer;

        PermissionGroupDefinitions = new Dictionary<string, PermissionGroupDefinition>();
        PermissionDefinitions = new Dictionary<string, PermissionDefinition>();
    }

    /// <summary>
    /// 使用数据库中的权限组和权限定义记录重建缓存。
    /// </summary>
    /// <param name="permissionGroupRecords">权限组记录。</param>
    /// <param name="permissionRecords">权限定义记录。</param>
    /// <returns>异步任务。</returns>
    public Task FillAsync(List<PermissionGroup> permissionGroupRecords, List<PermissionDefinitionRecord> permissionRecords)
    {
        PermissionGroupDefinitions.Clear();
        PermissionDefinitions.Clear();

        var context = new PermissionDefinitionContext(null!);

        foreach (var permissionGroupRecord in permissionGroupRecords)
        {
            var groupDisplayName = !string.IsNullOrWhiteSpace(permissionGroupRecord.LocalizationKey)
                ? LocalizableStringSerializer.Deserialize(permissionGroupRecord.LocalizationKey)
                : new FixedLocalizableString(permissionGroupRecord.DisplayName);

            var permissionGroup = context.AddGroup(permissionGroupRecord.Name, groupDisplayName);

            PermissionGroupDefinitions[permissionGroup.Name] = permissionGroup;

            foreach (var property in permissionGroupRecord.ExtraProperties)
            {
                permissionGroup[property.Key] = property.Value;
            }

            var permissionRecordsInThisGroup = permissionRecords.Where(p => p.GroupName == permissionGroup.Name);

            foreach (var permissionRecord in permissionRecordsInThisGroup.Where(x => x.ParentName == null))
            {
                AddPermissionRecursively(permissionGroup, permissionRecord, permissionRecords);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据名称获取权限定义。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <returns>权限定义不存在时返回 null。</returns>
    public PermissionDefinition? GetPermissionOrNull(string name)
    {
        return PermissionDefinitions.GetOrDefault(name);
    }

    /// <summary>
    /// 获取所有权限定义。
    /// </summary>
    /// <returns>权限定义列表。</returns>
    public IReadOnlyList<PermissionDefinition> GetPermissions()
    {
        return [.. PermissionDefinitions.Values];
    }

    /// <summary>
    /// 获取所有权限组定义。
    /// </summary>
    /// <returns>权限组定义列表。</returns>
    public IReadOnlyList<PermissionGroupDefinition> GetGroups()
    {
        return [.. PermissionGroupDefinitions.Values];
    }

    /// <summary>
    /// 递归添加权限定义及其子权限。
    /// </summary>
    /// <param name="permissionContainer">权限组或父级权限容器。</param>
    /// <param name="permissionRecord">当前权限记录。</param>
    /// <param name="allPermissionRecords">全部权限记录。</param>
    private void AddPermissionRecursively(ICanAddChildPermission permissionContainer,PermissionDefinitionRecord permissionRecord,List<PermissionDefinitionRecord> allPermissionRecords)
    {
        var permission = permissionContainer.AddPermission(
            permissionRecord.Name,
            !string.IsNullOrWhiteSpace(permissionRecord.LocalizationKey)
                ? LocalizableStringSerializer.Deserialize(permissionRecord.LocalizationKey)
                : new FixedLocalizableString(permissionRecord.DisplayName),
            permissionRecord.MultiTenancySide,
            permissionRecord.IsEnabled
        );

        PermissionDefinitions[permission.Name] = permission;

        if (!string.IsNullOrWhiteSpace(permissionRecord.Providers))
        {
            permission.Providers.AddRange(permissionRecord.Providers.Split(','));
        }

        if (!string.IsNullOrWhiteSpace(permissionRecord.StateCheckers))
        {
            var checkers = StateCheckerSerializer.DeserializeArray(permissionRecord.StateCheckers, permission);
            permission.StateCheckers.AddRange(checkers);
        }

        foreach (var property in permissionRecord.ExtraProperties)
        {
            permission[property.Key] = property.Value;
        }

        foreach (var subPermission in allPermissionRecords.Where(p => p.ParentName == permissionRecord.Name))
        {
            AddPermissionRecursively(permission, subPermission, allPermissionRecords);
        }
    }
}
