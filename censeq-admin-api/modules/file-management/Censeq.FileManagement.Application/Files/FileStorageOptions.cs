using OnceMi.AspNetCore.OSS;

namespace Censeq.FileManagement.Files;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string Provider { get; set; } = FileStorageProviderNames.Local;

    public long MaxFileSize { get; set; } = 20 * 1024 * 1024;

    public string[] ImageExtensions { get; set; } =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    ];

    public LocalFileStorageOptions Local { get; set; } = new();

    public OssFileStorageOptions Oss { get; set; } = new();
}

public class LocalFileStorageOptions
{
    public string BasePath { get; set; } = "uploads";
}

public class OssFileStorageOptions
{
    public bool Enabled { get; set; }

    public OSSProvider Provider { get; set; } = OSSProvider.Minio;

    public string Bucket { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string CustomHost { get; set; } = string.Empty;

    public bool IsEnableHttps { get; set; } = true;

    public bool IsEnableCache { get; set; } = true;
}

public static class FileStorageProviderNames
{
    public const string Local = "Local";

    public const string Oss = "Oss";
}
