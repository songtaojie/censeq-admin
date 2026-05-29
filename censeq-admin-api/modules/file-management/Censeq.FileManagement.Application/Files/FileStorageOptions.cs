using OnceMi.AspNetCore.OSS;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储全局配置。
/// </summary>
public class FileStorageOptions
{
    /// <summary>
    /// 配置文件中的根节点名称。
    /// </summary>
    public const string SectionName = "FileStorage";

    /// <summary>
    /// 默认文件存储提供器名称。
    /// </summary>
    public string Provider { get; set; } = FileStorageProviderNames.Local;

    /// <summary>
    /// 允许上传的最大文件大小，单位为字节。
    /// </summary>
    public long MaxFileSize { get; set; } = 20 * 1024 * 1024;

    /// <summary>
    /// 允许作为图片上传的文件扩展名。
    /// </summary>
    public string[] ImageExtensions { get; set; } =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    ];

    /// <summary>
    /// 本地文件存储配置。
    /// </summary>
    public LocalFileStorageOptions Local { get; set; } = new();

    /// <summary>
    /// OSS 文件存储配置。
    /// </summary>
    public OssFileStorageOptions Oss { get; set; } = new();
}

/// <summary>
/// 本地文件存储配置。
/// </summary>
public class LocalFileStorageOptions
{
    /// <summary>
    /// 文件在 wwwroot 下的基础相对路径。
    /// </summary>
    public string BasePath { get; set; } = "uploads";
}

/// <summary>
/// OSS 文件存储配置。
/// </summary>
public class OssFileStorageOptions
{
    /// <summary>
    /// 是否启用 OSS 存储。
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// OSS 提供商类型。
    /// </summary>
    public OSSProvider Provider { get; set; } = OSSProvider.Minio;

    /// <summary>
    /// 存储桶名称。
    /// </summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// 存储服务访问端点。
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 存储服务区域。
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥标识。
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// 访问密钥 Secret。
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 自定义访问域名。
    /// </summary>
    public string CustomHost { get; set; } = string.Empty;

    /// <summary>
    /// 是否使用 HTTPS。
    /// </summary>
    public bool IsEnableHttps { get; set; } = true;

    /// <summary>
    /// 是否启用 OSS 客户端缓存。
    /// </summary>
    public bool IsEnableCache { get; set; } = true;
}

/// <summary>
/// 文件存储提供器名称常量。
/// </summary>
public static class FileStorageProviderNames
{
    /// <summary>
    /// 本地文件存储。
    /// </summary>
    public const string Local = "Local";

    /// <summary>
    /// OSS 对象存储。
    /// </summary>
    public const string Oss = "Oss";
}
