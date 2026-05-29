using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器配置，描述 OSS/对象存储的连接参数和启用状态。
/// </summary>
public class FileProvider : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 所属租户标识，空值表示宿主侧配置。
    /// </summary>
    public Guid? TenantId { get; protected set; }

    /// <summary>
    /// 存储提供商名称，例如 Aliyun、QCloud 或 Minio。
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
    /// 访问密钥 Secret。
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
    /// 生成文件访问地址时是否使用 HTTPS。
    /// </summary>
    public bool IsEnableHttps { get; set; } = true;

    /// <summary>
    /// OSS 客户端是否启用缓存。
    /// </summary>
    public bool IsEnableCache { get; set; } = true;

    /// <summary>
    /// 该提供器配置是否启用。
    /// </summary>
    public bool IsEnable { get; set; } = true;

    /// <summary>
    /// 是否为默认存储提供器。
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 自定义文件访问域名。
    /// </summary>
    public string? CustomDomain { get; set; }

    /// <summary>
    /// 提供器选择排序值，数值越小优先级越高。
    /// </summary>
    public int OrderNo { get; set; } = 100;

    /// <summary>
    /// 配置备注。
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 面向展示的提供器名称。
    /// </summary>
    public string DisplayName => $"{Provider}-{BucketName}";

    /// <summary>
    /// OSS 服务实例缓存键。
    /// </summary>
    public string ConfigKey => $"{Provider}_{BucketName}_{Id:N}";

    protected FileProvider()
    {
    }

    /// <summary>
    /// 创建文件存储提供器配置。
    /// </summary>
    public FileProvider(Guid id, Guid? tenantId) : base(id)
    {
        TenantId = tenantId;
    }
}
