using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Censeq.Abp.Identity;

namespace Censeq.Abp.Identity.EntityFrameworkCore;

/// <summary>
/// �������ݿ�������
/// </summary>
[ConnectionStringName(CenseqIdentityDbProperties.ConnectionStringName)]
public class IdentityDbContext : AbpDbContext<IdentityDbContext>, IIdentityDbContext
{
    /// <summary>
    /// �û�
    /// </summary>
    public DbSet<IdentityUser> Users { get; set; }

    /// <summary>
    /// ��ɫ
    /// </summary>
    public DbSet<IdentityRole> Roles { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }

    /// <summary>
    /// ��֯
    /// </summary>
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }

    /// <summary>
    /// ��ȫ��־
    /// </summary>
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }

    /// <summary>
    /// �����û�
    /// </summary>
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    /// <summary>
    /// �û�ί��
    /// </summary>
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    /// <summary>
    /// �Ự
    /// </summary>
    public DbSet<IdentitySession> Sessions { get; set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="options"></param>
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {

    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureIdentity();
    }
}
