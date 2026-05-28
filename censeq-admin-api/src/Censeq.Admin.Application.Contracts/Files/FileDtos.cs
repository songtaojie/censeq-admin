using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.Admin.Files;

/// <summary>
/// 文件上传记录输出模型。
/// </summary>
public class FileRecordDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 文件所属租户。
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 文件上传人。
    /// </summary>
    public Guid? OwnerUserId { get; set; }

    /// <summary>
    /// 原始文件名。
    /// </summary>
    public string OriginalName { get; set; } = string.Empty;

    /// <summary>
    /// 存储文件名。
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件扩展名。
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// 文件 MIME 类型。
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// 相对存储路径。
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// 前端访问地址。
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小，单位字节。
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 文件内容哈希。
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// 业务分类。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 是否公开访问。
    /// </summary>
    public bool IsPublic { get; set; }
}

/// <summary>
/// 文件记录分页查询条件。
/// </summary>
public class GetFileRecordsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 文件名关键字。
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 文件业务分类。
    /// </summary>
    public string? Category { get; set; }
}
