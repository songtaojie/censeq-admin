using Volo.Abp.Application.Dtos;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件上传记录输出 DTO。
/// </summary>
public class FileRecordDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 所属租户标识。
    /// </summary>
    public Guid? TenantId { get; set; }

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
    /// 文件在存储中的相对路径。
    /// </summary>
    public string RelativePath { get; set; } = string.Empty;

    /// <summary>
    /// 可访问的文件地址。
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小，单位为字节。
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 文件内容哈希值。
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// 文件业务分类。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 是否为公开文件。
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 实际存储提供器名称。
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 物理存储实现名称，例如 Local 或 Oss。
    /// </summary>
    public string? StorageProvider { get; set; }

    /// <summary>
    /// OSS Bucket 名称。
    /// </summary>
    public string? BucketName { get; set; }
}

/// <summary>
/// 文件上传记录分页查询条件。
/// </summary>
public class GetFileRecordsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 文件名模糊搜索关键字。
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 文件业务分类。
    /// </summary>
    public string? Category { get; set; }
}

/// <summary>
/// 文件存储提供器输出 DTO。
/// </summary>
public class FileProviderDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 所属租户标识。
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 存储提供商名称。
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 存储桶名称。
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥标识。
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// 存储服务区域。
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// 存储服务访问端点。
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// 是否启用 HTTPS 访问地址。
    /// </summary>
    public bool IsEnableHttps { get; set; }

    /// <summary>
    /// OSS 客户端是否启用缓存。
    /// </summary>
    public bool IsEnableCache { get; set; }

    /// <summary>
    /// 该配置是否启用。
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 是否为默认存储提供器。
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 自定义文件访问域名。
    /// </summary>
    public string? CustomDomain { get; set; }

    /// <summary>
    /// 选择排序值。
    /// </summary>
    public int OrderNo { get; set; }

    /// <summary>
    /// 配置备注。
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 面向展示的提供器名称。
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// 文件存储提供器分页查询条件。
/// </summary>
public class GetFileProvidersInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 提供器、Bucket 或备注的模糊搜索关键字。
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 存储提供商名称。
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// 是否只查询启用或禁用的配置。
    /// </summary>
    public bool? IsEnable { get; set; }
}

/// <summary>
/// 创建或更新文件存储提供器配置的输入 DTO。
/// </summary>
public class CreateUpdateFileProviderDto
{
    /// <summary>
    /// 存储提供商名称。
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// 存储桶名称。
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥标识。
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// 访问密钥 Secret，更新时为空表示保留原值。
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// 存储服务区域。
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// 存储服务访问端点。
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// 生成访问地址时是否使用 HTTPS。
    /// </summary>
    public bool IsEnableHttps { get; set; } = true;

    /// <summary>
    /// OSS 客户端是否启用缓存。
    /// </summary>
    public bool IsEnableCache { get; set; } = true;

    /// <summary>
    /// 该配置是否启用。
    /// </summary>
    public bool IsEnable { get; set; } = true;

    /// <summary>
    /// 是否设置为默认存储提供器。
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 自定义文件访问域名。
    /// </summary>
    public string? CustomDomain { get; set; }

    /// <summary>
    /// 选择排序值。
    /// </summary>
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 配置备注。
    /// </summary>
    public string? Remark { get; set; }
}
