using Volo.Abp.Application.Dtos;

namespace Censeq.FileManagement.Files;

public class FileRecordDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? Hash { get; set; }
    public string? Category { get; set; }
    public bool IsPublic { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? BucketName { get; set; }
}

public class GetFileRecordsInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Category { get; set; }
}

public class FileProviderDto : FullAuditedEntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string? AccessKey { get; set; }
    public string? Region { get; set; }
    public string? Endpoint { get; set; }
    public bool IsEnableHttps { get; set; }
    public bool IsEnableCache { get; set; }
    public bool IsEnable { get; set; }
    public bool IsDefault { get; set; }
    public string? CustomDomain { get; set; }
    public int OrderNo { get; set; }
    public string? Remark { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class GetFileProvidersInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public string? Provider { get; set; }
    public bool? IsEnable { get; set; }
}

public class CreateUpdateFileProviderDto
{
    public string Provider { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? Region { get; set; }
    public string? Endpoint { get; set; }
    public bool IsEnableHttps { get; set; } = true;
    public bool IsEnableCache { get; set; } = true;
    public bool IsEnable { get; set; } = true;
    public bool IsDefault { get; set; }
    public string? CustomDomain { get; set; }
    public int OrderNo { get; set; } = 100;
    public string? Remark { get; set; }
}
