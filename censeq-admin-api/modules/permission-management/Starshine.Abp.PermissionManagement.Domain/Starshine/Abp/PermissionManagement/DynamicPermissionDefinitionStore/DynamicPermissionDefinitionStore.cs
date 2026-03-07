using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Censeq.Abp.PermissionManagement.Repositories;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Threading;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// ��̬Ȩ�޶���洢
/// </summary>
[Dependency(ReplaceServices = true)]
public class DynamicPermissionDefinitionStore : IDynamicPermissionDefinitionStore, ITransientDependency
{
    /// <summary>
    /// Ȩ����洢��
    /// </summary>
    protected IPermissionGroupDefinitionRecordRepository PermissionGroupRepository { get; }

    /// <summary>
    /// Ȩ�޴洢��
    /// </summary>
    protected IPermissionDefinitionRecordRepository PermissionRepository { get; }

    /// <summary>
    /// Ȩ�޶������л���
    /// </summary>
    protected IPermissionDefinitionSerializer PermissionDefinitionSerializer { get; }

    /// <summary>
    /// �洢����
    /// </summary>
    protected IDynamicPermissionDefinitionStoreInMemoryCache StoreCache { get; }

    /// <summary>
    /// �ֲ�ʽ����
    /// </summary>
    protected IDistributedCache DistributedCache { get; }

    /// <summary>
    /// �ֲ�ʽ��
    /// </summary>
    protected IAbpDistributedLock DistributedLock { get; }

    /// <summary>
    /// Ȩ�޹�������
    /// </summary>
    public PermissionManagementOptions PermissionManagementOptions { get; }

    /// <summary>
    /// �ֲ�ʽ��������
    /// </summary>
    protected AbpDistributedCacheOptions CacheOptions { get; }


    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="permissionGroupRepository"></param>
    /// <param name="permissionRepository"></param>
    /// <param name="permissionDefinitionSerializer"></param>
    /// <param name="storeCache"></param>
    /// <param name="distributedCache"></param>
    /// <param name="cacheOptions"></param>
    /// <param name="permissionManagementOptions"></param>
    /// <param name="distributedLock"></param>
    public DynamicPermissionDefinitionStore(
        IPermissionGroupDefinitionRecordRepository permissionGroupRepository,
        IPermissionDefinitionRecordRepository permissionRepository,
        IPermissionDefinitionSerializer permissionDefinitionSerializer,
        IDynamicPermissionDefinitionStoreInMemoryCache storeCache,
        IDistributedCache distributedCache,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IOptions<PermissionManagementOptions> permissionManagementOptions,
        IAbpDistributedLock distributedLock)
    {
        PermissionGroupRepository = permissionGroupRepository;
        PermissionRepository = permissionRepository;
        PermissionDefinitionSerializer = permissionDefinitionSerializer;
        StoreCache = storeCache;
        DistributedCache = distributedCache;
        DistributedLock = distributedLock;
        PermissionManagementOptions = permissionManagementOptions.Value;
        CacheOptions = cacheOptions.Value;
    }

    /// <summary>
    /// ��ȡȨ��
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual async Task<PermissionDefinition?> GetOrNullAsync(string name)
    {
        if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
        {
            return null;
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetPermissionOrNull(name);
        }
    }

    /// <summary>
    /// ��ȡȨ��
    /// </summary>
    /// <returns></returns>
    public virtual async Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync()
    {
        if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
        {
            return Array.Empty<PermissionDefinition>();
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetPermissions().ToImmutableList();
        }
    }

    /// <summary>
    /// ��ȡȨ����
    /// </summary>
    /// <returns></returns>
    public virtual async Task<IReadOnlyList<PermissionGroupDefinition>> GetGroupsAsync()
    {
        if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
        {
            return [];
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetGroups().ToImmutableList();
        }
    }

    /// <summary>
    /// ȷ�����漰ʱ����
    /// </summary>
    /// <returns></returns>
    protected virtual async Task EnsureCacheIsUptoDateAsync()
    {
        if (StoreCache.LastCheckTime.HasValue &&
            DateTime.Now.Subtract(StoreCache.LastCheckTime.Value).TotalSeconds < 30)
        {
            /* Ϊ���Ż������ǻ���΢�ӳٻ�ȡ���µ�Ȩ�� */
            return;
        }

        var stampInDistributedCache = await GetOrSetStampInDistributedCache();

        if (stampInDistributedCache == StoreCache.CacheStamp)
        {
            StoreCache.LastCheckTime = DateTime.Now;
            return;
        }

        await UpdateInMemoryStoreCache();

        StoreCache.CacheStamp = stampInDistributedCache;
        StoreCache.LastCheckTime = DateTime.Now;
    }

    /// <summary>
    /// �����ڴ�洢����
    /// </summary>
    /// <returns></returns>
    protected virtual async Task UpdateInMemoryStoreCache()
    {
        var permissionGroupRecords = await PermissionGroupRepository.GetListAsync();
        var permissionRecords = await PermissionRepository.GetListAsync();

        await StoreCache.FillAsync(permissionGroupRecords, permissionRecords);
    }

    /// <summary>
    /// ��ȡ�����÷ֲ�ʽ�����еı��
    /// </summary>
    /// <returns></returns>
    /// <exception cref="AbpException"></exception>
    protected virtual async Task<string> GetOrSetStampInDistributedCache()
    {
        var cacheKey = GetCommonStampCacheKey();

        var stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
        if (stampInDistributedCache != null)
        {
            return stampInDistributedCache;
        }

        await using (var commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(1)))
        {
            if (commonLockHandle == null)
            {
                /* This request will fail */
                throw new AbpException("�޷���ȡ����Ȩ�޶��幫����Ǽ��ķֲ�ʽ��!");
            }

            stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
            if (stampInDistributedCache != null)
            {
                return stampInDistributedCache;
            }

            stampInDistributedCache = Guid.NewGuid().ToString();

            await DistributedCache.SetStringAsync(cacheKey,
                stampInDistributedCache,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
                }
            );
        }

        return stampInDistributedCache;
    }

    /// <summary>
    /// ��ȡͨ�ô��ǻ�����Կ
    /// </summary>
    /// <returns></returns>
    protected virtual string GetCommonStampCacheKey()
    {
        return $"{CacheOptions.KeyPrefix}:InMemory:PermissionCacheStamp";
    }

    /// <summary>
    /// ��ȡͨ�÷ֲ�ʽ����Կ
    /// </summary>
    /// <returns></returns>
    protected virtual string GetCommonDistributedLockKey()
    {
        return $"{CacheOptions.KeyPrefix}:Permission:UpdateLock";
    }
}