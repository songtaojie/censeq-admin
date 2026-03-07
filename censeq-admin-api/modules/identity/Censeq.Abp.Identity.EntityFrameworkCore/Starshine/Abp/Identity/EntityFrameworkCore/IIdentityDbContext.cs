using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Censeq.Abp.Identity;

namespace Censeq.Abp.Identity.EntityFrameworkCore;

/// <summary>
/// �������ݿ�������
/// </summary>
[ConnectionStringName(StarshineIdentityDbProperties.ConnectionStringName)]
public interface IIdentityDbContext : IEfCoreDbContext
{
    /// <summary>
    /// �û�
    /// </summary>
    DbSet<IdentityUser> Users { get; }

    /// <summary>
    /// ��ɫ
    /// </summary>
    DbSet<IdentityRole> Roles { get; }

    /// <summary>
    /// ����
    /// </summary>
    DbSet<IdentityClaimType> ClaimTypes { get; }

    /// <summary>
    /// ��֯
    /// </summary>
    DbSet<OrganizationUnit> OrganizationUnits { get; }

    /// <summary>
    /// ��ȫ��־
    /// </summary>
    DbSet<IdentitySecurityLog> SecurityLogs { get; }

    /// <summary>
    /// �����û�
    /// </summary>
    DbSet<IdentityLinkUser> LinkUsers { get; }

    /// <summary>
    /// �û�ί��
    /// </summary>
    DbSet<IdentityUserDelegation> UserDelegations { get; }

    /// <summary>
    /// �Ự
    /// </summary>
    DbSet<IdentitySession> Sessions { get; }
}
