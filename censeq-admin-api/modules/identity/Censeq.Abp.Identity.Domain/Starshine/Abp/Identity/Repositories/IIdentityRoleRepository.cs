using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Abp.Identity;

/// <summary>
/// ���ݽ�ɫ�洢��
/// </summary>
public interface IIdentityRoleRepository : IBasicRepository<IdentityRole, Guid>
{
    /// <summary>
    /// ͨ���첽�������Ҿ���ָ���淶�����ƵĽ�ɫ��
    /// </summary>
    /// <param name="normalizedRoleName">Ҫ���ҵĹ淶����ɫ���ơ�</param>
    /// <param name="includeDetails">�Ƿ������ϸ</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>A <see cref="Task{TResult}"/> ���ҵĽ����</returns>
    Task<IdentityRole?> FindByNormalizedNameAsync(string normalizedRoleName,bool includeDetails = true,CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�б�����
    /// </summary>
    /// <param name="sorting"></param>
    /// <param name="maxResultCount"></param>
    /// <param name="skipCount"></param>
    /// <param name="filter"></param>
    /// <param name="includeDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IdentityRoleWithUserCount>> GetListWithUserCountAsync(string? sorting = null, int maxResultCount = int.MaxValue,int skipCount = 0,string? filter = null,bool includeDetails = false,CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ�б�����
    /// </summary>
    /// <param name="sorting"></param>
    /// <param name="maxResultCount"></param>
    /// <param name="skipCount"></param>
    /// <param name="filter"></param>
    /// <param name="includeDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IdentityRole>> GetListAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// ��ȡ�б�����
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IdentityRole>> GetListAsync(IEnumerable<Guid> ids,CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡĬ�Ͻ�ɫ����
    /// </summary>
    /// <param name="includeDetails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IdentityRole>> GetDefaultOnesAsync(bool includeDetails = false,CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��������������
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="claimType"></param>
    /// <param name="autoSave"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveClaimFromAllRolesAsync(string claimType,bool autoSave = false,CancellationToken cancellationToken = default);
}
