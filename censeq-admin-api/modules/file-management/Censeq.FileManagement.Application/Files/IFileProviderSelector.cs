namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器选择器。
/// </summary>
public interface IFileProviderSelector
{
    /// <summary>
    /// 获取当前可用的默认 OSS 提供器配置。
    /// </summary>
    Task<FileProvider?> GetDefaultAsync();

    /// <summary>
    /// 根据提供商和 Bucket 查找可用的 OSS 提供器配置。
    /// </summary>
    Task<FileProvider?> FindAsync(string? provider, string? bucketName);
}
