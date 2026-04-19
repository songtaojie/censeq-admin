using Censeq.PermissionManagement.Entities;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.SimpleStateChecking;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限定义序列化
/// </summary>
public class PermissionDefinitionSerializer : IPermissionDefinitionSerializer, ITransientDependency
{
    /// <summary>
    /// 状态检查序列化
    /// </summary>
    protected ISimpleStateCheckerSerializer StateCheckerSerializer { get; }

    /// <summary>
    /// guid生成器
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }

    /// <summary>
    /// 本地化
    /// </summary>
    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

    /// <summary>
    /// 本地化工厂，用于解析实际文本存入数据库
    /// </summary>
    protected IStringLocalizerFactory StringLocalizerFactory { get; }

    /// <summary>
    /// 权限定义序列化
    /// </summary>
    public PermissionDefinitionSerializer(
        IGuidGenerator guidGenerator,
        ISimpleStateCheckerSerializer stateCheckerSerializer,
        ILocalizableStringSerializer localizableStringSerializer,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        StateCheckerSerializer = stateCheckerSerializer;
        LocalizableStringSerializer = localizableStringSerializer;
        GuidGenerator = guidGenerator;
        StringLocalizerFactory = stringLocalizerFactory;
    }

    /// <summary>
    /// 将 ILocalizableString 解析为实际文本，回退到序列化 key
    /// </summary>
    private string ResolveDisplayName(ILocalizableString? displayName, string fallback)
    {
        if (displayName == null)
        {
            return fallback;
        }

        try
        {
            // 使用中文（zh-Hans）解析，存入可读文本
            using (CultureHelper.Use(new CultureInfo("zh-Hans")))
            {
                var text = displayName.Localize(StringLocalizerFactory);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
        }
        catch
        {
            // 解析失败时回退到序列化 key
        }

        return LocalizableStringSerializer.Serialize(displayName) ?? fallback;
    }

    /// <summary>
    /// 序列化
    /// </summary>
    public async Task<(PermissionGroup[], PermissionDefinitionRecord[])> SerializeAsync(IEnumerable<PermissionGroupDefinition> permissionGroups)
    {
        var permissionGroupRecords = new List<PermissionGroup>(permissionGroups.Count());
        var permissionRecords = new List<PermissionDefinitionRecord>();

        foreach (var permissionGroup in permissionGroups)
        {
            permissionGroupRecords.Add(await SerializeAsync(permissionGroup));

            foreach (var permission in permissionGroup.GetPermissionsWithChildren())
            {
                permissionRecords.Add(await SerializeAsync(permission, permissionGroup));
            }
        }

        return (permissionGroupRecords.ToArray(), permissionRecords.ToArray());
    }

    /// <summary>
    /// 序列化权限组
    /// </summary>
    public Task<PermissionGroup> SerializeAsync(PermissionGroupDefinition permissionGroup)
    {
        var localizationKey = LocalizableStringSerializer.Serialize(permissionGroup.DisplayName);
        var displayName = ResolveDisplayName(permissionGroup.DisplayName, permissionGroup.Name);

        var permissionGroupRecord = new PermissionGroup(
            GuidGenerator.Create(),
            permissionGroup.Name,
            displayName,
            localizationKey);

        foreach (var property in permissionGroup.Properties)
        {
            permissionGroupRecord.SetProperty(property.Key, property.Value);
        }

        return Task.FromResult(permissionGroupRecord);
    }

    /// <summary>
    /// 序列化权限项
    /// </summary>
    public Task<PermissionDefinitionRecord> SerializeAsync(PermissionDefinition permission, PermissionGroupDefinition? permissionGroup)
    {
        var localizationKey = LocalizableStringSerializer.Serialize(permission.DisplayName);
        var displayName = ResolveDisplayName(permission.DisplayName, permission.Name);

        var permissionRecord = new PermissionDefinitionRecord(GuidGenerator.Create())
        {
            GroupName = permissionGroup?.Name ?? string.Empty,
            DisplayName = displayName,
            LocalizationKey = localizationKey,
            Name = permission.Name,
            ParentName = permission.Parent?.Name,
            IsEnabled = permission.IsEnabled,
            MultiTenancySide = permission.MultiTenancySide,
            Providers = SerializeProviders(permission.Providers),
            StateCheckers = SerializeStateCheckers(permission.StateCheckers)
        };

        foreach (var property in permission.Properties)
        {
            permissionRecord.SetProperty(property.Key, property.Value);
        }

        return Task.FromResult(permissionRecord);
    }

    /// <summary>
    /// 序列化提供程序
    /// </summary>
    protected virtual string? SerializeProviders(ICollection<string> providers)
    {
        return providers == null || providers.Count == 0 ? null : providers.JoinAsString(",");
    }

    /// <summary>
    /// 序列化状态检查
    /// </summary>
    protected virtual string? SerializeStateCheckers(List<ISimpleStateChecker<PermissionDefinition>> stateCheckers)
    {
        return StateCheckerSerializer.Serialize(stateCheckers);
    }
}