using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Json.SystemTextJson.Modifiers;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement;

/// <summary>
/// 静态权限保存器。
/// 将代码中定义的静态权限同步到数据库，供动态权限定义存储和管理端读取。
/// </summary>
public class StaticPermissionSaver : IStaticPermissionSaver, ITransientDependency
{
    /// <summary>
    /// ABP 静态权限定义存储。
    /// </summary>
    protected IStaticPermissionDefinitionStore StaticStore { get; }

    /// <summary>
    /// 权限组仓储。
    /// </summary>
    protected IPermissionGroupRepository PermissionGroupRepository { get; }

    /// <summary>
    /// 权限定义记录仓储。
    /// </summary>
    protected IPermissionDefinitionRecordRepository PermissionRepository { get; }

    /// <summary>
    /// 权限定义序列化器。
    /// </summary>
    protected IPermissionDefinitionSerializer PermissionSerializer { get; }

    /// <summary>
    /// 分布式缓存。
    /// </summary>
    protected IDistributedCache Cache { get; }

    /// <summary>
    /// 当前应用信息。
    /// </summary>
    protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }

    /// <summary>
    /// 分布式锁。
    /// </summary>
    protected IAbpDistributedLock DistributedLock { get; }

    /// <summary>
    /// ABP 权限配置。
    /// </summary>
    protected AbpPermissionOptions PermissionOptions { get; }

    /// <summary>
    /// 取消令牌提供器。
    /// </summary>
    protected ICancellationTokenProvider CancellationTokenProvider { get; }

    /// <summary>
    /// 分布式缓存配置。
    /// </summary>
    protected AbpDistributedCacheOptions CacheOptions { get; }

    /// <summary>
    /// 工作单元管理器。
    /// </summary>
    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    /// <summary>
    /// 初始化静态权限保存器。
    /// </summary>
    /// <param name="staticStore">静态权限定义存储。</param>
    /// <param name="permissionGroupRepository">权限组仓储。</param>
    /// <param name="permissionRepository">权限定义记录仓储。</param>
    /// <param name="permissionSerializer">权限定义序列化器。</param>
    /// <param name="cache">分布式缓存。</param>
    /// <param name="cacheOptions">分布式缓存配置。</param>
    /// <param name="applicationInfoAccessor">当前应用信息。</param>
    /// <param name="distributedLock">分布式锁。</param>
    /// <param name="permissionOptions">ABP 权限配置。</param>
    /// <param name="cancellationTokenProvider">取消令牌提供器。</param>
    /// <param name="unitOfWorkManager">工作单元管理器。</param>
    public StaticPermissionSaver(
        IStaticPermissionDefinitionStore staticStore,
        IPermissionGroupRepository permissionGroupRepository,
        IPermissionDefinitionRecordRepository permissionRepository,
        IPermissionDefinitionSerializer permissionSerializer,
        IDistributedCache cache,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IApplicationInfoAccessor applicationInfoAccessor,
        IAbpDistributedLock distributedLock,
        IOptions<AbpPermissionOptions> permissionOptions,
        ICancellationTokenProvider cancellationTokenProvider,
        IUnitOfWorkManager unitOfWorkManager)
    {
        UnitOfWorkManager = unitOfWorkManager;
        StaticStore = staticStore;
        PermissionGroupRepository = permissionGroupRepository;
        PermissionRepository = permissionRepository;
        PermissionSerializer = permissionSerializer;
        Cache = cache;
        ApplicationInfoAccessor = applicationInfoAccessor;
        DistributedLock = distributedLock;
        CancellationTokenProvider = cancellationTokenProvider;
        PermissionOptions = permissionOptions.Value;
        CacheOptions = cacheOptions.Value;
    }

    /// <summary>
    /// 保存静态权限定义。
    /// 使用应用级锁避免同一应用重复同步，使用公共锁保护跨应用共享的权限表更新。
    /// </summary>
    /// <returns>异步任务。</returns>
    /// <exception cref="AbpException">无法获取公共分布式锁时抛出。</exception>
    public async Task SaveAsync()
    {
        await using var applicationLockHandle = await DistributedLock.TryAcquireAsync(
            GetApplicationDistributedLockKey()
        );

        if (applicationLockHandle == null)
        {
            /* Another application instance is already doing it */
            return;
        }

        /* NOTE: This can be further optimized by using 4 cache values for:
         * Groups, permissions, deleted groups and deleted permissions.
         * But the code would be more complex. This is enough for now.
         */

        var cacheKey = GetApplicationHashCacheKey();
        var cachedHash = await Cache.GetStringAsync(cacheKey, CancellationTokenProvider.Token);

        var (permissionGroupRecords, permissionRecords) = await PermissionSerializer.SerializeAsync(
            await StaticStore.GetGroupsAsync()
        );

        var currentHash = CalculateHash(
            permissionGroupRecords,
            permissionRecords,
            PermissionOptions.DeletedPermissionGroups,
            PermissionOptions.DeletedPermissions
        );

        if (cachedHash == currentHash)
        {
            return;
        }

        await using (var commonLockHandle = await DistributedLock.TryAcquireAsync(
                         GetCommonDistributedLockKey(),
                         TimeSpan.FromMinutes(5)))
        {
            if (commonLockHandle == null)
            {
                /* It will re-try */
                throw new AbpException("Could not acquire distributed lock for saving static permissions!");
            }

            using (var unitOfWork = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                try
                {
                    var hasChangesInGroups = await UpdateChangedPermissionGroupsAsync(permissionGroupRecords);
                    var hasChangesInPermissions = await UpdateChangedPermissionsAsync(permissionRecords);

                    if (hasChangesInGroups || hasChangesInPermissions)
                    {
                        await Cache.SetStringAsync(
                            GetCommonStampCacheKey(),
                            Guid.NewGuid().ToString(),
                            new DistributedCacheEntryOptions
                            {
                                SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
                            },
                            CancellationTokenProvider.Token
                        );
                    }
                }
                catch
                {
                    try
                    {
                        await unitOfWork.RollbackAsync();
                    }
                    catch
                    {
                        /* ignored */
                    }

                    throw;
                }

                await unitOfWork.CompleteAsync();
            }
        }

        await Cache.SetStringAsync(
            cacheKey,
            currentHash,
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
            },
            CancellationTokenProvider.Token
        );
    }

    /// <summary>
    /// 同步权限组记录的新增、变更和删除。
    /// </summary>
    /// <param name="permissionGroupRecords">序列化后的权限组记录。</param>
    /// <returns>存在数据库变更时返回 true。</returns>
    private async Task<bool> UpdateChangedPermissionGroupsAsync(
        IEnumerable<PermissionGroup> permissionGroupRecords)
    {
        var newRecords = new List<PermissionGroup>();
        var changedRecords = new List<PermissionGroup>();
        var list = await PermissionGroupRepository.GetListAsync();
        var permissionGroupRecordsInDatabase = list.ToDictionary(x => x.Name);

        foreach (var permissionGroupRecord in permissionGroupRecords)
        {
            var permissionGroupRecordInDatabase =
                permissionGroupRecordsInDatabase.GetOrDefault(permissionGroupRecord.Name);
            if (permissionGroupRecordInDatabase == null)
            {
                /* New group */
                newRecords.Add(permissionGroupRecord);
                continue;
            }

            if (permissionGroupRecord.HasSameData(permissionGroupRecordInDatabase))
            {
                /* Not changed */
                continue;
            }

            /* Changed */
            permissionGroupRecordInDatabase.Patch(permissionGroupRecord);
            changedRecords.Add(permissionGroupRecordInDatabase);
        }

        /* Deleted */
        var deletedRecords = PermissionOptions.DeletedPermissionGroups.Any()
            ? permissionGroupRecordsInDatabase.Values
                .Where(x => PermissionOptions.DeletedPermissionGroups.Contains(x.Name))
                .ToArray()
            : Array.Empty<PermissionGroup>();

        if (newRecords.Any())
        {
            await PermissionGroupRepository.InsertManyAsync(newRecords);
        }

        if (changedRecords.Any())
        {
            await PermissionGroupRepository.UpdateManyAsync(changedRecords);
        }

        if (deletedRecords.Any())
        {
            await PermissionGroupRepository.DeleteManyAsync(deletedRecords);
        }

        return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
    }

    /// <summary>
    /// 同步权限定义记录的新增、变更和删除。
    /// 被标记删除的权限组会连带删除其下权限定义。
    /// </summary>
    /// <param name="permissionRecords">序列化后的权限定义记录。</param>
    /// <returns>存在数据库变更时返回 true。</returns>
    private async Task<bool> UpdateChangedPermissionsAsync(
        IEnumerable<PermissionDefinitionRecord> permissionRecords)
    {
        var newRecords = new List<PermissionDefinitionRecord>();
        var changedRecords = new List<PermissionDefinitionRecord>();

        var permissionRecordsInDatabase = (await PermissionRepository.GetListAsync())
            .ToDictionary(x => x.Name);

        foreach (var permissionRecord in permissionRecords)
        {
            var permissionRecordInDatabase = permissionRecordsInDatabase.GetOrDefault(permissionRecord.Name);
            if (permissionRecordInDatabase == null)
            {
                /* New group */
                newRecords.Add(permissionRecord);
                continue;
            }

            if (permissionRecord.HasSameData(permissionRecordInDatabase))
            {
                /* Not changed */
                continue;
            }

            /* Changed */
            permissionRecordInDatabase.Patch(permissionRecord);
            changedRecords.Add(permissionRecordInDatabase);
        }

        /* Deleted */
        var deletedRecords = new List<PermissionDefinitionRecord>();

        if (PermissionOptions.DeletedPermissions.Any())
        {
            deletedRecords.AddRange(
                permissionRecordsInDatabase.Values
                    .Where(x => PermissionOptions.DeletedPermissions.Contains(x.Name))
            );
        }

        if (PermissionOptions.DeletedPermissionGroups.Any())
        {
            deletedRecords.AddIfNotContains(
                permissionRecordsInDatabase.Values
                    .Where(x => PermissionOptions.DeletedPermissionGroups.Contains(x.GroupName))
            );
        }

        if (newRecords.Any())
        {
            await PermissionRepository.InsertManyAsync(newRecords);
        }

        if (changedRecords.Any())
        {
            await PermissionRepository.UpdateManyAsync(changedRecords);
        }

        if (deletedRecords.Any())
        {
            await PermissionRepository.DeleteManyAsync(deletedRecords);
        }

        return newRecords.Any() || changedRecords.Any() || deletedRecords.Any();
    }

    /// <summary>
    /// 获取应用级分布式锁键。
    /// </summary>
    /// <returns>分布式锁键。</returns>
    private string GetApplicationDistributedLockKey()
    {
        return $"{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpPermissionUpdateLock";
    }

    /// <summary>
    /// 获取跨应用共享的分布式锁键。
    /// </summary>
    /// <returns>分布式锁键。</returns>
    private string GetCommonDistributedLockKey()
    {
        return $"{CacheOptions.KeyPrefix}_Common_AbpPermissionUpdateLock";
    }

    /// <summary>
    /// 获取当前应用静态权限哈希缓存键。
    /// </summary>
    /// <returns>缓存键。</returns>
    private string GetApplicationHashCacheKey()
    {
        return $"{CacheOptions.KeyPrefix}_{ApplicationInfoAccessor.ApplicationName}_AbpPermissionsHash";
    }

    /// <summary>
    /// 获取公共权限定义缓存标记键。
    /// </summary>
    /// <returns>缓存键。</returns>
    private string GetCommonStampCacheKey()
    {
        return $"{CacheOptions.KeyPrefix}_AbpInMemoryPermissionCacheStamp";
    }

    /// <summary>
    /// 根据权限组、权限定义和删除配置计算静态权限哈希。
    /// 哈希用于判断本应用的静态权限是否需要重新同步。
    /// </summary>
    /// <param name="permissionGroupRecords">权限组记录。</param>
    /// <param name="permissionRecords">权限定义记录。</param>
    /// <param name="deletedPermissionGroups">已删除权限组配置。</param>
    /// <param name="deletedPermissions">已删除权限配置。</param>
    /// <returns>静态权限哈希。</returns>
    private static string CalculateHash(
        PermissionGroup[] permissionGroupRecords,
        PermissionDefinitionRecord[] permissionRecords,
        IEnumerable<string> deletedPermissionGroups,
        IEnumerable<string> deletedPermissions)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    new AbpIgnorePropertiesModifiers<PermissionGroup, Guid>().CreateModifyAction(x => x.Id),
                    new AbpIgnorePropertiesModifiers<PermissionDefinitionRecord, Guid>().CreateModifyAction(x => x.Id)
                }
            }
        };

        var stringBuilder = new StringBuilder();

        stringBuilder.Append("PermissionGroupRecords:");
        stringBuilder.AppendLine(JsonSerializer.Serialize(permissionGroupRecords, jsonSerializerOptions));

        stringBuilder.Append("PermissionRecords:");
        stringBuilder.AppendLine(JsonSerializer.Serialize(permissionRecords, jsonSerializerOptions));

        stringBuilder.Append("DeletedPermissionGroups:");
        stringBuilder.AppendLine(deletedPermissionGroups.JoinAsString(","));

        stringBuilder.Append("DeletedPermission:");
        stringBuilder.Append(deletedPermissions.JoinAsString(","));

        return stringBuilder
            .ToString()
            .ToMd5();
    }
}
