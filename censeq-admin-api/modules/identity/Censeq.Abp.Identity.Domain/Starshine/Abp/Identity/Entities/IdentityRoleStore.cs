using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Censeq.Abp.Identity;

/// <summary>
/// Ϊ��ɫ�����־��Դ洢����ʵ����
/// </summary>
public class IdentityRoleStore : IRoleStore<IdentityRole>, IRoleClaimStore<IdentityRole>, ITransientDependency
{
    /// <summary>
    /// ��ɫ�洢��
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// ��־��¼
    /// </summary>
    protected ILogger<IdentityRoleStore> Logger { get; }
    /// <summary>
    /// Guid������
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }

    /// <summary>
    ///���� <see cref="IdentityRoleStore"/> ����ʵ����
    /// </summary>
    public IdentityRoleStore(IIdentityRoleRepository roleRepository,ILogger<IdentityRoleStore> logger,
        IGuidGenerator guidGenerator,IdentityErrorDescriber? describer = null)
    {
        RoleRepository = roleRepository;
        Logger = logger;
        GuidGenerator = guidGenerator;
        ErrorDescriber = describer ?? new IdentityErrorDescriber();
    }

    /// <summary>
    /// ��ȡ�����õ�ǰ�����������κδ���� <see cref="IdentityErrorDescriber"/>��
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    /// ��ȡ������һ����־��ָʾ�ڵ��� CreateAsync��UpdateAsync �� DeleteAsync ���Ƿ�Ӧ�������ġ�
    /// </summary>
    /// <value>
    /// ���Ӧ�Զ��������ģ���Ϊ True������Ϊ false��
    /// </value>
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>
    /// ��Ϊ�첽�������̵��д����½�ɫ��
    /// </summary>
    /// <param name="role">�ڴ洢�д����Ľ�ɫ.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> ��ʾ�첽��ѯ�� <see cref="IdentityResult"/>.</returns>
    public virtual async Task<IdentityResult> CreateAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        await RoleRepository.InsertAsync(role, AutoSaveChanges, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// ��Ϊ�첽��������store�еĽ�ɫ��
    /// </summary>
    /// <param name="role">store��Ҫ���µĽ�ɫ��</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ.</param>
    /// <returns>A <see cref="Task{TResult}"/> ��ʾ�첽��ѯ�� <see cref="IdentityResult"/>.</returns>
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
            Logger.LogError(ex,"���½�ɫʧ��"); //����һЩ AbpHandledException �¼�
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// ��Ϊ�첽�����Ӵ洢��ɾ����ɫ
    /// </summary>
    /// <param name="role">store��Ҫɾ���Ľ�ɫ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> �����첽��ѯ�� <see cref="IdentityResult"/>��</returns>
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
            Logger.LogError(ex,"ɾ����ɫʧ��");  //����һЩ AbpHandledException �¼�
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// ��Ϊ�첽�����Ӵ洢�л�ȡ��ɫ�� ID��
    /// </summary>
    /// <param name="role">Ӧ������ ID �Ľ�ɫ.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> ������ɫ ID.</returns>
    public virtual Task<string> GetRoleIdAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        return Task.FromResult(role.Id.ToString());
    }

    /// <summary>
    /// ��Ϊ�첽�����Ӵ洢�л�ȡ��ɫ�����ơ�
    /// </summary>
    /// <param name="role">Ӧ���������ƵĽ�ɫ.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> ������ɫ�����ơ�</returns>
    public virtual Task<string?> GetRoleNameAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        return Task.FromResult<string?>(role.Name);
    }

    /// <summary>
    /// ��store�н�ɫ����������Ϊ�첽������
    /// </summary>
    /// <param name="role">Ӧ���ý�ɫ����.</param>
    /// <param name="roleName">��ɫ����.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>The <see cref="Task"/> �����첽������</returns>
    public virtual Task SetRoleNameAsync([NotNull] IdentityRole role, string? roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        Check.NotNull(roleName, nameof(roleName));
        role.ChangeName(roleName);
        return Task.CompletedTask;
    }

    /// <summary>
    /// ͨ���첽�������Ҿ���ָ��ID�Ľ�ɫ��
    /// </summary>
    /// <param name="id">Ҫ���ҵĽ�ɫ ID��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> ���ҵĽ��.</returns>
    public virtual Task<IdentityRole?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return RoleRepository.FindAsync(Guid.Parse(id), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ͨ���첽�������Ҿ���ָ���淶�����ƵĽ�ɫ��
    /// </summary>
    /// <param name="normalizedName">Ҫ���ҵĹ淶����ɫ���ơ�</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> ���ҵĽ����</returns>
    public virtual Task<IdentityRole?> FindByNameAsync([NotNull] string normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(normalizedName, nameof(normalizedName));
        return RoleRepository.FindByNormalizedNameAsync(normalizedName, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ���첽�����ķ�ʽ��ȡ��ɫ�Ĺ淶�����ơ�
    /// </summary>
    /// <param name="role">Ӧ������淶�����ƵĽ�ɫ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>������ɫ���Ƶ� <see cref="Task{TResult}"/>��</returns>
    public virtual Task<string?> GetNormalizedRoleNameAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        return Task.FromResult<string?>(role.NormalizedName);
    }

    /// <summary>
    /// ����ɫ�Ĺ淶����������Ϊ�첽������
    /// </summary>
    /// <param name="role">Ӧ���ù淶�����ƵĽ�ɫ��</param>
    /// <param name="normalizedName">Ҫ���õĹ淶������</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>The <see cref="Task"/> �����첽������</returns>
    public virtual Task SetNormalizedRoleNameAsync([NotNull] IdentityRole role, string? normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        Check.NotNull(normalizedName, nameof(normalizedName));
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    /// <summary>
    /// �ͷ�stores
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// ͨ���첽������ȡ��ָ�� <paramref name="role"/> ������������
    /// </summary>
    /// <param name="role">Ӧ�����������Ľ�ɫ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>���������ɫ�������� <see cref="Task{TResult}"/>��</returns>
    public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] IdentityRole role, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        await RoleRepository.EnsureCollectionLoadedAsync(role, r => r.Claims, cancellationToken);
        return role.Claims.Select(c => c.ToClaim()).ToList();
    }

    /// <summary>
    /// ���Ӹ���ָ�� <paramref name="claim"/> �� <paramref name="role"/>��
    /// </summary>
    /// <param name="role">Ҫ���������Ľ�ɫ��</param>
    /// <param name="claim">���ӵ���ɫ��������</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task AddClaimAsync([NotNull] IdentityRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(role, nameof(role));
        Check.NotNull(claim, nameof(claim));
        await RoleRepository.EnsureCollectionLoadedAsync(role, r => r.Claims, cancellationToken);
        role.AddClaim(GuidGenerator, claim);
    }

    /// <summary>
    /// ��ָ���� <paramref name="role"/> ��ɾ�������� <paramref name="claim"/>��
    /// </summary>
    /// <param name="role">Ҫɾ�������Ľ�ɫ��</param>
    /// <param name="claim">Ҫ��ӽ�ɫ���Ƴ���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task RemoveClaimAsync([NotNull] IdentityRole role, [NotNull] Claim claim, CancellationToken cancellationToken = default)
    {
        Check.NotNull(role, nameof(role));
        Check.NotNull(claim, nameof(claim));
        await RoleRepository.EnsureCollectionLoadedAsync(role, r => r.Claims, cancellationToken);
        role.RemoveClaim(claim);
    }
}
