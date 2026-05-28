using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.FileManagement.Files;

public class FileRecord : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }

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

    protected FileRecord()
    {
    }

    public FileRecord(Guid id, Guid? tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}
