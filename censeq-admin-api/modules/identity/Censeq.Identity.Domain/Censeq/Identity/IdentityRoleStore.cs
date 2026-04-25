using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Censeq.Identity;

/// <summary>
/// 创建角色持久化存储的新实例。
/// </summary>
public class IdentityRoleStore :
    IRoleStore<IdentityRole>,
    IRoleClaimStore<IdentityRole>,
    ITransientDependency
{
    /// <summary>
    /// I身份角色仓储
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// ILogger<Identity角色Store>
    /// </summary>
    protected ILogger<IdentityRoleStore> Logger { get; }
    /// <summary>
    /// IGuidGenerator
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }

    /// <summary>
    /// 初始化 <see cref="IdentityRoleStore"/> 类的新实例。
    /// </summary>
    public IdentityRoleStore(
        IIdentityRoleRepository roleRepository,
        ILogger<IdentityRoleStore> logger,
        IGuidGenerator guidGenerator,
        IdentityErrorDescriber? describer = null)
    {
        RoleRepository = roleRepository;
        Logger = logger;
        GuidGenerator = guidGenerator;

        ErrorDescriber = describer ?? new IdentityErrorDescriber();
    }

    /// <summary>
    /// 获取或设置当前操作发生错误时使用的 <see cref="IdentityErrorDescriber"/>。
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    /// 获取或设置一个标志，指示是否在调用 CreateAsync、UpdateAsync 和 DeleteAsync 后自动持久化更改。
    /// </summary>
    /// <value>
    /// 如果应自动持久化更改则为 true，否则为 false。
    /// </value>
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>
    /// 以异步操作在存储中创建新角色。
    /// </summary>
    /// <param name="role">要在存储中创建的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步查询的 <see cref="IdentityResult"/> 的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IdentityResult> CreateAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        await RoleRepository.InsertAsync(role, AutoSaveChanges, cancellationToken);

        return IdentityResult.Success;
    }

    /// <summary>
    /// 以异步操作更新存储中的角色。
    /// </summary>
    /// <param name="role">要在存储中更新的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步查询的 <see cref="IdentityResult"/> 的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IdentityResult> UpdateAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        try
        {
            await RoleRepository.UpdateAsync(role, AutoSaveChanges, cancellationToken);
        }
        catch (AbpDbConcurrencyException ex)
        {
            Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// 以异步操作从存储中删除角色。
    /// </summary>
    /// <param name="role">要从存储中删除的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步查询的 <see cref="IdentityResult"/> 的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IdentityResult> DeleteAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        try
        {
            await RoleRepository.DeleteAsync(role, AutoSaveChanges, cancellationToken);
        }
        catch (AbpDbConcurrencyException ex)
        {
            Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// 以异步操作从存储中获取角色的 ID。
    /// </summary>
    /// <param name="role">要返回其 ID 的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含角色 ID 的 <see cref="Task{TResult}"/>。</returns>
    public virtual Task<string> GetRoleIdAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        return Task.FromResult(role.Id.ToString());
    }

    /// <summary>
    /// 以异步操作从存储中获取角色的名称。
    /// </summary>
    /// <param name="role">要返回其名称的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含角色名称的 <see cref="Task{TResult}"/>。</returns>
    public virtual Task<string?> GetRoleNameAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        return Task.FromResult<string?>(role.Name);
    }

    /// <summary>
    /// 以异步操作设置存储中角色的名称。
    /// </summary>
    /// <param name="role">要设置其名称的角色。</param>
    /// <param name="roleName">角色的名称。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetRoleNameAsync([NotNull] IdentityRole role, string? roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        role.ChangeName(roleName!);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 以异步操作查找具有指定 ID 的角色。
    /// </summary>
    /// <param name="id">要查找的角色 ID。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>查找结果的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IdentityRole?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await RoleRepository.FindAsync(Guid.Parse(id), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 以异步操作查找具有指定标准化名称的角色。
    /// </summary>
    /// <param name="normalizedName">要查找的标准化角色名称。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>查找结果的 <see cref="Task{TResult}"/>。</returns>
    public virtual Task<IdentityRole?> FindByNameAsync([NotNull] string normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(normalizedName, nameof(normalizedName));

        return RoleRepository.FindByNormalizedNameAsync(normalizedName, cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// 以异步操作获取角色的标准化名称。
    /// </summary>
    /// <param name="role">要获取其标准化名称的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含角色名称的 <see cref="Task{TResult}"/>。</returns>
    public virtual Task<string?> GetNormalizedRoleNameAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        return Task.FromResult<string?>(role.NormalizedName);
    }

    /// <summary>
    /// 以异步操作设置角色的标准化名称。
    /// </summary>
    /// <param name="role">要设置其标准化名称的角色。</param>
    /// <param name="normalizedName">要设置的标准化名称。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetNormalizedRoleNameAsync([NotNull] IdentityRole role, string? normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        role.NormalizedName = normalizedName!;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 释放存储所占用的资源。
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// 以异步操作获取与指定 <paramref name="role"/> 关联的声明。
    /// </summary>
    /// <param name="role">要获取其声明的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含授予角色的声明的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));

        await RoleRepository.EnsureCollectionLoadedAsync(role, r => r.Claims, cancellationToken);

        return role.Claims.Select(c => c.ToClaim()).ToList();
    }

    /// <summary>
    /// 将指定的 <paramref name="claim"/> 添加到指定的 <paramref name="role"/>。
    /// </summary>
    /// <param name="role">要添加声明的角色。</param>
    /// <param name="claim">要添加到角色的声明。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task AddClaimAsync([NotNull] IdentityRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(role, nameof(role));
        Check.NotNull(claim, nameof(claim));

        await RoleRepository.EnsureCollectionLoadedAsync(role, r => r.Claims, cancellationToken);

        role.AddClaim(GuidGenerator, claim);
    }

    /// <summary>
    /// 从指定的 <paramref name="role"/> 中移除指定的 <paramref name="claim"/>。
    /// </summary>
    /// <param name="role">要移除声明的角色。</param>
    /// <param name="claim">要从角色移除的声明。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task RemoveClaimAsync([NotNull] IdentityRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default)
    {
        Check.NotNull(role, nameof(role));
        Check.NotNull(claim, nameof(claim));

        await RoleRepository.EnsureCollectionLoadedAsync(role, r => r.Claims, cancellationToken);

        role.RemoveClaim(claim);
    }
}
