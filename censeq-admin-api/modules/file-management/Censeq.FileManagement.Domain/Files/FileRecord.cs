using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件上传记录，保存文件元数据、访问地址、归属用户和实际存储位置。
/// </summary>
public class FileRecord : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 所属租户标识，空值表示宿主侧文件。
    /// </summary>
    public Guid? TenantId { get; protected set; }

    /// <summary>
    /// 上传文件的用户标识。
    /// </summary>
    public Guid? OwnerUserId { get; set; }

    /// <summary>
    /// 用户上传时的原始文件名。
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 不含扩展名的文件名。
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名，包含前导点。
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// 文件 MIME 类型。
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 文件在存储提供器中的相对路径。
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// 可供客户端访问的文件地址。
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小，单位为字节。
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 文件内容哈希值，用于去重或完整性校验。
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// 业务分类，例如 common 或 avatar。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 是否作为公开文件保存。
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 实际使用的存储提供器名称。
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 物理存储实现名称，例如 Local 或 Oss。
    /// </summary>
    public string? StorageProvider { get; set; }

    /// <summary>
    /// OSS 场景下文件所在的 Bucket 名称。
    /// </summary>
    public string? BucketName { get; set; }

    protected FileRecord()
    {
    }

    /// <summary>
    /// 创建文件上传记录。
    /// </summary>
    public FileRecord(Guid id, Guid? tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}
