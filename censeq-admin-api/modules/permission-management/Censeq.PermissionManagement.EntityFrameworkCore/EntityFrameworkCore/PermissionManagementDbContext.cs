using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 权限管理 DbContext。
/// 负责权限管理模块实体的 EF Core 映射入口。
/// </summary>
[ConnectionStringName(CenseqPermissionManagementDbProperties.ConnectionStringName)]
public class PermissionManagementDbContext : AbpDbContext<PermissionManagementDbContext>, IPermissionManagementDbContext
{
    /// <summary>
    /// 权限组记录。
    /// </summary>
    public DbSet<PermissionGroup> PermissionGroups { get; set; }

    /// <summary>
    /// 权限定义记录。
    /// </summary>
    public DbSet<PermissionDefinitionRecord> Permissions { get; set; }

    /// <summary>
    /// 权限授予记录。
    /// </summary>
    public DbSet<PermissionGrant> PermissionGrants { get; set; }

    /// <summary>
    /// 初始化权限管理 DbContext。
    /// </summary>
    /// <param name="options">DbContext 配置选项。</param>
    public PermissionManagementDbContext(DbContextOptions<PermissionManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 配置权限管理实体模型。
    /// </summary>
    /// <param name="builder">模型构建器。</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigurePermissionManagement();
    }
}
