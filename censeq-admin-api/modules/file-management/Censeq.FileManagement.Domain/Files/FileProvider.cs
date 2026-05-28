using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.FileManagement.Files;

public class FileProvider : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }

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

    public string DisplayName => $"{Provider}-{BucketName}";

    public string ConfigKey => $"{Provider}_{BucketName}_{Id:N}";

    protected FileProvider()
    {
    }

    public FileProvider(Guid id, Guid? tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}
