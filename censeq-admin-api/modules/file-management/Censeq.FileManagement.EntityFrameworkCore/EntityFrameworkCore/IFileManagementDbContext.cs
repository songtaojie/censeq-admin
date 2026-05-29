using Censeq.FileManagement.Files;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.FileManagement.EntityFrameworkCore;

/// <summary>
/// 文件管理模块数据库上下文契约。
/// </summary>
[ConnectionStringName(ConnectionStrings.DefaultConnectionStringName)]
public interface IFileManagementDbContext : IEfCoreDbContext
{
    /// <summary>
    /// 文件上传记录集合。
    /// </summary>
    DbSet<FileRecord> FileRecords { get; set; }

    /// <summary>
    /// 文件存储提供器配置集合。
    /// </summary>
    DbSet<FileProvider> FileProviders { get; set; }
}
