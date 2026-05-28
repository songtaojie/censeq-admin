using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Admin.Files;

/// <summary>
/// 文件上传记录，用于保存物理文件和业务访问地址之间的映射关系。
/// </summary>
public class FileRecord : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户标识，平台级公共文件为空。
    /// </summary>
    public Guid? TenantId { get; protected set; }

    /// <summary>
    /// 上传用户标识，用于追踪文件归属。
    /// </summary>
    public Guid? OwnerUserId { get; set; }

    /// <summary>
    /// 用户上传时的原始文件名。
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 存储时使用的文件基础名，不包含扩展名。
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名，包含点号。
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// 文件 MIME 类型。
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 相对于 wwwroot 的物理存储路径。
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// 前端可访问的相对 URL。
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小，单位字节。
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 文件内容哈希，用于后续去重或完整性校验。
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// 业务分类，例如 avatar、common。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 是否按公开资源处理。
    /// </summary>
    public bool IsPublic { get; set; }

    protected FileRecord()
    {
    }

    /// <summary>
    /// 创建文件记录并绑定当前租户。
    /// </summary>
    public FileRecord(Guid id, Guid? tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}
